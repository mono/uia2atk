#include <stdio.h>
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
                                                        Gailtoplevel            *text);
static gboolean         hello_toplevel_hide_event_watcher (GSignalInvocationHint *ihint,
                                                        guint                   n_param_values,
                                                        const GValue            *param_values,
                                                        gpointer                data);
static gboolean         hello_toplevel_show_event_watcher (GSignalInvocationHint *ihint,
                                                        guint                   n_param_values,
                                                        const GValue            *param_values,
                                                        gpointer                data);
 
 
/* Misc: IDEM

static void      _hello_toplevel_remove_child            (Hellotoplevel           *toplevel,
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
                                   "HelloToplevel", &tinfo, 0);
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
  GList *l;
  guint signal_id;
  
  l = toplevel->window_list = mytk_window_list_toplevels ();

  //assign destroy callbacks and delete invalid windows
  while (l)
    {
      //originally, in gail, the data is a window (which is also a widget):
      //MytkWindow *window;
      //MytkWidget *widget;
      //window = MYTK_WINDOW (l->data);
      //widget = MYTK_WIDGET (window);
      MytkWidget *window = MYTK_WIDGET (l->data);
      
      if (!window 
          //reasonable facts for discarding, but disabled for now:
          //|| !GTK_WIDGET_VISIBLE (widget) ||
          //window->parent ||
         )
        {
          GList *temp_l  = l->next;

          toplevel->window_list = g_list_delete_link (toplevel->window_list, l);
          l = temp_l;
        }
      else
        {
//          g_signal_connect (G_OBJECT (window), 
//                            "destroy",
//                            G_CALLBACK (hello_toplevel_window_destroyed),
//                            toplevel);
          l = l->next;
        }
    }

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
  AtkObject* atk_obj;

  toplevel = HELLO_TOPLEVEL (obj);
  ptr = g_list_nth_data (toplevel->window_list, i);
  if (!ptr)
    return NULL;
  widget = MYTK_WIDGET (ptr);
  atk_obj = mytk_widget_get_accessible (widget);

  g_object_ref (atk_obj);
  return atk_obj;
}

/*
 * Window destroy events on GtkWindow cause a child to be removed
 * from the toplevel
 *
static void
gail_toplevel_window_destroyed (GtkWindow    *window,
                                GailToplevel *toplevel)
{
  _gail_toplevel_remove_child (toplevel, window);
}


 *
 * Common code used by destroy and hide events on GtkWindow
 *
static void
_gail_toplevel_remove_child (GailToplevel *toplevel, 
                             GtkWindow    *window)
{
  AtkObject *atk_obj = ATK_OBJECT (toplevel);
  GList *l;
  guint window_count = 0;
  AtkObject *child;

  if (toplevel->window_list)
    {
        GtkWindow *tmp_window;

        // Must loop through them all
        for (l = toplevel->window_list; l; l = l->next)
        {
          tmp_window = GTK_WINDOW (l->data);

          if (window == tmp_window)
            {
              // Remove the window from the window_list & emit the signal
              toplevel->window_list = g_list_remove (toplevel->window_list,
                                                     l->data);
              child = gtk_widget_get_accessible (GTK_WIDGET (window));
              g_signal_emit_by_name (atk_obj, "children-changed::remove",
                                     window_count, 
                                     child, NULL);
              atk_object_set_parent (child, NULL);
              break;
            }

          window_count++;
        }
    }
}

*/