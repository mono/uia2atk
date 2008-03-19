
#include <stdlib.h>
#include <string.h>
#include "hellotoplevel.h"
#include "mytkwidget.h"

static void             hello_toplevel_class_init        (HelloToplevelClass      *klass);
static void             hello_toplevel_object_init       (HelloToplevel           *toplevel);
static void             hello_toplevel_object_finalize   (GObject                *obj);

/* atkobject.h */

static gint             hello_toplevel_get_n_children    (AtkObject              *obj);
static AtkObject*       hello_toplevel_ref_child         (AtkObject              *obj,
                                                        gint                    i);
static AtkObject*       hello_toplevel_get_parent        (AtkObject              *obj);


/* Callbacks:
   We comment this section because it has reference to GtkWindow class
   and because it goes beyond the atkHelloWorld purpose


static void             hello_toplevel_window_destroyed  (GtkWindow              *window,
                                                        GailToplevel            *text);
static gboolean         hello_toplevel_hide_event_watcher (GSignalInvocationHint *ihint,
                                                        guint                   n_param_values,
                                                        const GValue            *param_values,
                                                        gpointer                data);
static gboolean         hello_toplevel_show_event_watcher (GSignalInvocationHint *ihint,
                                                        guint                   n_param_values,
                                                        const GValue            *param_values,
                                                        gpointer                data);
 
 
/* Misc: IDEM

static void      _hello_toplevel_remove_child            (HelloToplevel           *toplevel,
                                                        GtkWindow               *window);
static gboolean  is_attached_menu_window                (GtkWidget              *widget);
static gboolean  is_combo_window                        (GtkWidget              *widget);

*/

static gpointer parent_class = NULL;

GType
hello_toplevel_get_type (void)
{
  static GType type = 0;

  if (!type)
    {
      static const GTypeInfo tinfo =
        {
          sizeof (HelloToplevelClass),
          (GBaseInitFunc) NULL, /* base init */
          (GBaseFinalizeFunc) NULL, /* base finalize */
          (GClassInitFunc) hello_toplevel_class_init, /* class init */
          (GClassFinalizeFunc) NULL, /* class finalize */
          NULL, /* class data */
          sizeof (HelloToplevel), /* instance size */
          0, /* nb preallocs */
          (GInstanceInitFunc) hello_toplevel_object_init, /* instance init */
          NULL /* value table */
        };

      type = g_type_register_static (ATK_TYPE_OBJECT,
                                   "GailToplevel", &tinfo, 0);
    }

  return type;
}

AtkObject*
hello_toplevel_new (void)
{
  GObject *object;
  AtkObject *accessible;

  object = g_object_new (HELLO_TYPE_TOPLEVEL, NULL);
  g_return_val_if_fail ((object != NULL), NULL);

  accessible = ATK_OBJECT (object);
  accessible->role = ATK_ROLE_APPLICATION;
  accessible->name = g_get_prgname();
  accessible->accessible_parent = NULL;

  return accessible;
}

static void
hello_toplevel_class_init (HelloToplevelClass *klass)
{
  AtkObjectClass *class = ATK_OBJECT_CLASS(klass);
  GObjectClass *g_object_class = G_OBJECT_CLASS(klass);

  parent_class = g_type_class_peek_parent (klass);

  class->get_n_children = hello_toplevel_get_n_children;
  class->ref_child = hello_toplevel_ref_child;
  class->get_parent = hello_toplevel_get_parent;

  g_object_class->finalize = hello_toplevel_object_finalize;
}

static void
hello_toplevel_object_init (HelloToplevel *toplevel)
{
  //commented lines are because they depend on GTK
  //FIXME: use my own test-toolkit
  /*GtkWindow *window;
  //GtkWidget *widget;
  GList *l;
  guint signal_id;
  
  l = toplevel->window_list = gtk_window_list_toplevels ();

  while (l)
    {
      window = GTK_WINDOW (l->data);
      widget = GTK_WIDGET (window);
      if (!window || 
          !GTK_WIDGET_VISIBLE (widget) ||
          is_attached_menu_window (widget) ||
          GTK_WIDGET (window)->parent ||
          GTK_IS_PLUG (window))
        {
          GList *temp_l  = l->next;

          toplevel->window_list = g_list_delete_link (toplevel->window_list, l);
          l = temp_l;
        }
      else
        {
          g_signal_connect (G_OBJECT (window), 
                            "destroy",
                            G_CALLBACK (hello_toplevel_window_destroyed),
                            toplevel);
          l = l->next;
        }
    }

  gtk_type_class (GTK_TYPE_WINDOW);

  signal_id  = g_signal_lookup ("show", GTK_TYPE_WINDOW);
  g_signal_add_emission_hook (signal_id, 0,
    hello_toplevel_show_event_watcher, toplevel, (GDestroyNotify) NULL);

  signal_id  = g_signal_lookup ("hide", GTK_TYPE_WINDOW);
  g_signal_add_emission_hook (signal_id, 0,
    hello_toplevel_hide_event_watcher, toplevel, (GDestroyNotify) NULL);
    
  */
}

static void
hello_toplevel_object_finalize (GObject *obj)
{
  HelloToplevel *toplevel = HELLO_TOPLEVEL (obj);

  if (toplevel->window_list)
    g_list_free (toplevel->window_list);

  G_OBJECT_CLASS (parent_class)->finalize (obj);
}

static AtkObject*
hello_toplevel_get_parent (AtkObject *obj)
{
    return NULL;
}

static gint
hello_toplevel_get_n_children (AtkObject *obj)
{
  HelloToplevel *toplevel = HELLO_TOPLEVEL (obj);

  gint rc = g_list_length (toplevel->window_list);
  return rc;
}

static AtkObject*
hello_toplevel_ref_child (AtkObject *obj,
                         gint      i)
{
  HelloToplevel *toplevel;
  gpointer ptr;
  MytkWidget *widget;
  AtkObject *atk_obj;

  toplevel = HELLO_TOPLEVEL (obj);
  ptr = g_list_nth_data (toplevel->window_list, i);
  if (!ptr)
    return NULL;
  widget = MYTK_WIDGET (ptr);
  atk_obj = mytk_widget_get_accessible (widget);

  g_object_ref (atk_obj);
  return atk_obj;
}
