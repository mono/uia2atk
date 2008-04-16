#include <stdlib.h>
#include <string.h>
#include <stdio.h>
#include "helloutil.h"
#include "hellochild.h"
#include "mytkwidget.h"


static void		hello_util_class_init			(HelloUtilClass		*klass);

/* atkutil.h */
static AtkObject*	hello_util_get_root			(void);
static G_CONST_RETURN gchar *hello_util_get_toolkit_name		(void);
static G_CONST_RETURN gchar *hello_util_get_toolkit_version      (void);

/* hellomisc/AtkMisc */
static void		hello_misc_class_init			(HelloMiscClass		*klass);

/* Misc */

static AtkObject* root = NULL;
//static guint key_snooper_id = 0;
static gint listener_idx = 1;

typedef struct _HelloUtilListenerInfo HelloUtilListenerInfo;

struct _HelloUtilListenerInfo
{
   gint key;
   guint signal_id;
   gulong hook_id;
};

static GHashTable *listener_list = NULL;

GType
hello_util_get_type (void)
{
  static GType type = 0;

  if (!type)
  {
    static const GTypeInfo tinfo =
    {
      sizeof (HelloUtilClass),
      (GBaseInitFunc) NULL, /* base init */
      (GBaseFinalizeFunc) NULL, /* base finalize */
      (GClassInitFunc) hello_util_class_init, /* class init */
      (GClassFinalizeFunc) NULL, /* class finalize */
      NULL, /* class data */
      sizeof (HelloUtil), /* instance size */
      0, /* nb preallocs */
      (GInstanceInitFunc) NULL, /* instance init */
      NULL /* value table */
    };

    type = g_type_register_static (ATK_TYPE_UTIL,
                                   "HelloUtil", &tinfo, 0);
  }
  return type;
}


static guint
add_listener (GSignalEmissionHook listener,
              const gchar         *object_type,
              const gchar         *signal,
              const gchar         *hook_data)
{
  GType type;
  guint signal_id;
  gint  rc = 0;

  type = g_type_from_name (object_type);
  if (type)
    {
      signal_id  = g_signal_lookup (signal, type);
      if (signal_id > 0)
        {
          HelloUtilListenerInfo *listener_info;

          rc = listener_idx;

          listener_info = g_malloc(sizeof(HelloUtilListenerInfo));
          listener_info->key = listener_idx;
          listener_info->hook_id =
                          g_signal_add_emission_hook (signal_id, 0, listener,
			        		      g_strdup (hook_data),
			        		      (GDestroyNotify) g_free);
          listener_info->signal_id = signal_id;

	  g_hash_table_insert(listener_list, &(listener_info->key), listener_info);
          listener_idx++;
        }
      else
        {
          g_warning("Invalid signal type %s\n", signal);
        }
    }
  else
    {
      g_warning("Invalid object type %s\n", object_type);
    }
  return rc;
}

static guint
hello_util_add_global_event_listener (GSignalEmissionHook listener,
				     const gchar *event_type)
{
  guint rc = 0;
  gchar **split_string;

  //split_string[0]: toolkit
  //            [1]: class/interface
  //            [2]: event type
  // example: Gtk:AtkObject:children-changed
  split_string = g_strsplit (event_type, ":", 3);

  g_warning(g_strdup_printf("add global event listener, event_type: %s", event_type));

  if (split_string)
    {
      rc = add_listener (listener, split_string[1], split_string[2], event_type);

      g_strfreev (split_string);
    }

  return rc;
}

static void
hello_util_remove_global_event_listener (guint remove_listener)
{
  if (remove_listener > 0)
  {
    HelloUtilListenerInfo *listener_info;
    gint tmp_idx = remove_listener;

    listener_info = (HelloUtilListenerInfo *)
      g_hash_table_lookup(listener_list, &tmp_idx);

    if (listener_info != NULL)
      {
        /* Hook id of 0 and signal id of 0 are invalid */
        if (listener_info->hook_id != 0 && listener_info->signal_id != 0)
          {
            /* Remove the emission hook */
            g_signal_remove_emission_hook(listener_info->signal_id,
              listener_info->hook_id);

            /* Remove the element from the hash */
            g_hash_table_remove(listener_list, &tmp_idx);
          }
        else
          {
            g_warning("Invalid listener hook_id %ld or signal_id %d\n",
              listener_info->hook_id, listener_info->signal_id);
          }
      }
    else
      {
        g_warning("No listener with the specified listener id %d", 
          remove_listener);
      }
  }
  else
  {
    g_warning("Invalid listener_id %d", remove_listener);
  }
}


static void
_listener_info_destroy (gpointer data)
{
   g_free(data);
}

static void	 
hello_util_class_init (HelloUtilClass *klass)
{
  AtkUtilClass *atk_class;
  gpointer data;

  data = g_type_class_peek (ATK_TYPE_UTIL);
  atk_class = ATK_UTIL_CLASS (data);

  atk_class->add_global_event_listener =
    hello_util_add_global_event_listener;
  atk_class->remove_global_event_listener =
    hello_util_remove_global_event_listener;
//  atk_class->add_key_event_listener =
//    hello_util_add_key_event_listener;
//  atk_class->remove_key_event_listener =
//    hello_util_remove_key_event_listener;
  atk_class->get_root = hello_util_get_root;
  atk_class->get_toolkit_name = hello_util_get_toolkit_name;
  atk_class->get_toolkit_version = hello_util_get_toolkit_version;

  listener_list = g_hash_table_new_full(g_int_hash, g_int_equal, NULL, 
     _listener_info_destroy);
}


static AtkObject*
hello_util_get_root (void)
{
  if (!root)
  {
    root = hello_toplevel_new();
  }

  return root;
}

static G_CONST_RETURN gchar *
hello_util_get_toolkit_name (void)
{
  return "HELLO";
}

static G_CONST_RETURN gchar *
hello_util_get_toolkit_version (void)
{
  return "1.1";
}

GType
hello_misc_get_type (void)
{
  static GType type = 0;

  if (!type)
  {
    static const GTypeInfo tinfo =
    {
      sizeof (HelloMiscClass),
      (GBaseInitFunc) NULL, /* base init */
      (GBaseFinalizeFunc) NULL, /* base finalize */
      (GClassInitFunc) hello_misc_class_init, /* class init */
      (GClassFinalizeFunc) NULL, /* class finalize */
      NULL, /* class data */
      sizeof (HelloMisc), /* instance size */
      0, /* nb preallocs */
      (GInstanceInitFunc) NULL, /* instance init */
      NULL /* value table */
    };

    type = g_type_register_static (ATK_TYPE_MISC,
                                   "HelloMisc", &tinfo, 0);
  }
  return type;
}


static void hello_misc_threads_enter (AtkMisc *misc)
{
//  GDK_THREADS_ENTER ();
}

static void hello_misc_threads_leave (AtkMisc *misc)
{
//  GDK_THREADS_LEAVE ();
}


static void	 
hello_misc_class_init (HelloMiscClass *klass)
{
  AtkMiscClass *miscclass = ATK_MISC_CLASS (klass);
  miscclass->threads_enter =
    hello_misc_threads_enter;
  miscclass->threads_leave =
    hello_misc_threads_leave;
  atk_misc_instance = g_object_new (HELLO_TYPE_MISC, NULL);
}
