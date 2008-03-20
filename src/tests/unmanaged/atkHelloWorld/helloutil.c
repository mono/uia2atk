#include <stdlib.h>
#include <string.h>
#include <stdio.h>
#include "helloutil.h"


static void		hello_util_class_init			(HelloUtilClass		*klass);

/* atkutil.h */
static AtkObject*	hello_util_get_root			(void);
static G_CONST_RETURN gchar *hello_util_get_toolkit_name		(void);
static G_CONST_RETURN gchar *hello_util_get_toolkit_version      (void);

/* hellomisc/AtkMisc */
static void		hello_misc_class_init			(HelloMiscClass		*klass);

/* Misc */

static AtkObject* root = NULL;
static guint key_snooper_id = 0;

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

static void	 
hello_util_class_init (HelloUtilClass *klass)
{
  AtkUtilClass *atk_class;
  gpointer data;

  data = g_type_class_peek (ATK_TYPE_UTIL);
  atk_class = ATK_UTIL_CLASS (data);

//  atk_class->add_global_event_listener =
//    hello_util_add_global_event_listener;
//  atk_class->remove_global_event_listener =
//    hello_util_remove_global_event_listener;
//  atk_class->add_key_event_listener =
//    hello_util_add_key_event_listener;
//  atk_class->remove_key_event_listener =
//    hello_util_remove_key_event_listener;
  atk_class->get_root = hello_util_get_root;
  atk_class->get_toolkit_name = hello_util_get_toolkit_name;
  atk_class->get_toolkit_version = hello_util_get_toolkit_version;

//  listener_list = g_hash_table_new_full(g_int_hash, g_int_equal, NULL, 
//     _listener_info_destroy);
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

static void
_listener_info_destroy (gpointer data)
{
   g_free(data);
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
