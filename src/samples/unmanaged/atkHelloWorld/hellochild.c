#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "hellochild.h"
#include "mytkwidget.h"

static void             hello_child_class_init        (HelloChildClass      *klass);
static void             hello_child_object_init       (HelloChild           *child);
static void             hello_child_object_finalize   (GObject              *obj);

/* atkobject.h */

static gint             hello_child_get_n_children    (AtkObject              *obj);
static AtkObject*       hello_child_ref_child         (AtkObject              *obj,
                                                        gint                    i);
static AtkObject*       hello_child_get_parent        (AtkObject              *obj);


static gpointer parent_class = NULL;



static gint hello_child_get_caret_offset(AtkText * text)
{
  HelloChild *obj = HELLO_CHILD(text);
  return 0;
}

static void atk_text_interface_init (AtkTextIface * iface)
{
    g_return_if_fail (iface != NULL);
    iface->get_caret_offset = hello_child_get_caret_offset;
}


GType hello_child_get_type(void)
{
  static GType type = 0;

  if (!type)
    {
      static const GTypeInfo tinfo = {
        sizeof(HelloChildClass),
        (GBaseInitFunc) NULL,	// base init
        (GBaseFinalizeFunc) hello_child_object_finalize,	// base finalize
        (GClassInitFunc) hello_child_class_init,	// class init
        (GClassFinalizeFunc) NULL,	// class finalize
        NULL,		// class data
        sizeof(HelloChild),	// instance size
        0,			// nb preallocs
        (GInstanceInitFunc) NULL,	// instance init
        NULL		// value table
      };

      static const GInterfaceInfo atk_text_info = {
        (GInterfaceInitFunc) atk_text_interface_init,
        (GInterfaceFinalizeFunc) NULL,
        NULL
      };

      type = g_type_register_static(atk_object_get_type(),
                                    "HelloChild", &tinfo, (GTypeFlags) 0);
      g_warning("well built!!!!!!!!!!!!!!!!!!!!!");

      g_type_add_interface_static (type, ATK_TYPE_TEXT, &atk_text_info);
    }
    else
    {
	    g_warning("built already!");
    }
  
  return type;
}

AtkObject*
hello_child_new (gchar* name)
{
  GObject *object;
  AtkObject *accessible;

  object = g_object_new (HELLO_TYPE_CHILD, NULL);
  g_return_val_if_fail ((object != NULL), NULL);

  accessible = ATK_OBJECT (object);
  accessible->role = ATK_ROLE_WINDOW;
  accessible->name = g_strdup(name);
  accessible->accessible_parent = NULL;

  return accessible;
}

static void
hello_child_class_init (HelloChildClass *klass)
{
  AtkObjectClass *class = ATK_OBJECT_CLASS(klass);
  GObjectClass *g_object_class = G_OBJECT_CLASS(klass);

  parent_class = g_type_class_peek_parent (klass);

  class->get_n_children = hello_child_get_n_children;
  class->ref_child = hello_child_ref_child;
  class->get_parent = hello_child_get_parent;

  g_object_class->finalize = hello_child_object_finalize;
}

static void
hello_child_object_init (HelloChild *child)
{

}

static void
hello_child_object_finalize (GObject *obj)
{
  HelloChild *child = HELLO_CHILD (obj);
  AtkObject* accessible = ATK_OBJECT (obj);

  //FIXME: isn't this public?
  //atk_object_finalize(accessible);
  G_OBJECT_CLASS (parent_class)->finalize (obj);
}

static AtkObject*
hello_child_get_parent (AtkObject *obj)
{
    return NULL;
}

static gint
hello_child_get_n_children (AtkObject *obj)
{
  //no grandchildren
  return 0;
}

static AtkObject*
hello_child_ref_child (AtkObject *obj,
                          gint      i)
{
  // this should not be called if we don't have grandchildren
  g_assert_not_reached();
}

