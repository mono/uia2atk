#include "atk/atk.h"
#include "hello.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <termios.h>
#include <unistd.h>
#include "helloutil.h"

#define NUM_CHILDREN 0
/* The below line isn't really right -- normally gtk will build the path */
#define ATK_BRIDGE_PATH "/usr/lib/gtk-2.0/modules/libatk-bridge.so"


static int hello_initialized = FALSE;


static void
hello_accessibility_module_init (void)
{
  if (hello_initialized)
    {
      return;
    }
  hello_initialized = TRUE;
  
  /* Initialize the HelloUtility classes */
  g_type_class_unref (g_type_class_ref (HELLO_TYPE_UTIL));
  g_type_class_unref (g_type_class_ref (HELLO_TYPE_MISC));
}

static void
load_atk_bridge_gmodule(void)
{
  GModule *bridge;
  void (*gnome_accessibility_module_init)();
  
  bridge = g_module_open(ATK_BRIDGE_PATH, G_MODULE_BIND_LOCAL|G_MODULE_BIND_LAZY);

  if (!bridge)
  {
    fprintf(stderr, "Couldn't load atk-bridge.so\n");
    exit(1);
  }

  if (!g_module_symbol(bridge, "gnome_accessibility_module_init", (gpointer *)&gnome_accessibility_module_init))
  {
    fprintf(stderr, "Couldn't find gnome_accessibility_module_init\n");    
    exit(1);
  }

  (*gnome_accessibility_module_init)();
}

//simulate the creation of toplevel windows
void
start_program_gui(void)
{
  GList *l = mytk_window_list_toplevels ();
  if ((l) && (g_list_length(l) > 0))
  {
    return;
  }
  int i = 0;
  for (i = 0; i < NUM_CHILDREN; i++)
  {
    mytk_add_one_top_level_window(g_strdup_printf("TopLevel %d", i));
  }
}

main(int argc, char *argv[])
{
  //this is originally in gtk_init ( http://library.gnome.org/devel/glib/stable/glib-Miscellaneous-Utility-Functions.html#g-get-prgname )
  g_set_prgname(argv[0]);

  GMainLoop *mainloop;
  struct termios current_termios;
  g_type_init();

  start_program_gui();

  hello_accessibility_module_init();
  load_atk_bridge_gmodule();

  mainloop = g_main_loop_new (NULL, FALSE);

  g_main_loop_run (mainloop);
  return 0;
}
