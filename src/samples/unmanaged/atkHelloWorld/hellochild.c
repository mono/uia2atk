#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "hellochild.h"

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
  //HelloChild *obj = HELLO_CHILD(text);
  return 3;
}

static gchar* hello_child_get_text_at_offset 
                   (AtkText         *text,
                    gint            offset,
                    AtkTextBoundary boundary_type,
                    gint            *start_offset,
                    gint            *end_offset)
{
  g_warning ("hello_child_get_text_at_offset function called!");
  //HelloChild *obj = HELLO_CHILD(text);
  *start_offset = offset;
  *end_offset = 2;
  return g_strdup("lol");
}

static gchar*
hello_child_get_text (AtkText *text,
                      gint    start_pos,
                      gint    end_pos)
{
  g_warning ("hello_child_get_text function called!");
  return g_strdup("hey");
}

static gchar*
hello_child_get_text_before_offset (AtkText         *text,
				    gint            offset,
				    AtkTextBoundary boundary_type,
				    gint            *start_offset,
				    gint            *end_offset)
{
  g_warning ("hello_child_get_text_before_offset function called!");
  //HelloChild *obj = HELLO_CHILD(text);
  *start_offset = offset;
  *end_offset = 2;
  return g_strdup("lol");
}

static gchar*
hello_child_get_text_after_offset (AtkText         *text,
				   gint            offset,
				   AtkTextBoundary boundary_type,
				   gint            *start_offset,
				   gint            *end_offset)
{
  g_warning ("hello_child_get_text_before_offset function called!");
  //HelloChild *obj = HELLO_CHILD(text);
  *start_offset = offset;
  *end_offset = 2;
  return "lol";
}

static gint
hello_child_get_character_count (AtkText *text)
{
  g_warning ("hello_child_get_character_count function called!");
  return 3;
}

static void
hello_child_get_character_extents (AtkText      *text,
				   gint         offset,
		                   gint         *x,
                    		   gint 	*y,
                                   gint 	*width,
                                   gint 	*height,
			           AtkCoordType coords)
{
  g_warning ("hello_child_get_character_extents function called!");
  *x = 5;
  *y = 7;
  *width = 10;
  *height = 20;
  return;
} 

static gint 
hello_child_get_offset_at_point (AtkText      *text,
                                 gint         x,
                                 gint         y,
			         AtkCoordType coords)
{
  g_warning ("hello_child_get_offset_at_point function called!");
  return 3; 
}

static AtkAttributeSet*
hello_child_get_run_attributes (AtkText        *text,
                                gint 	      offset,
                                gint 	      *start_offset,
	                        gint	      *end_offset)
{
  g_warning ("hello_child_get_run_attributes function called!");
  return NULL;
}

static AtkAttributeSet*
hello_child_get_default_attributes (AtkText        *text)
{
  g_warning ("hello_child_get_default_attributes function called!");
  return NULL;
}

static gunichar 
hello_child_get_character_at_offset (AtkText	         *text,
                                     gint	         offset)
{
  g_warning ("hello_child_get_character_at_offset function called!");
  return 'c';
}


static void
hello_child_atk_text_interface_init (AtkTextIface *iface)
{
  g_warning ("setting up ginterface AtkText");
  iface->get_text = hello_child_get_text;
  iface->get_character_at_offset = hello_child_get_character_at_offset;
  iface->get_text_before_offset = hello_child_get_text_before_offset;
  iface->get_text_at_offset = hello_child_get_text_at_offset;
  iface->get_text_after_offset = hello_child_get_text_after_offset;
  iface->get_character_count = hello_child_get_character_count;
  iface->get_character_extents = hello_child_get_character_extents;
  iface->get_offset_at_point = hello_child_get_offset_at_point;
  iface->get_run_attributes = hello_child_get_run_attributes;
  iface->get_default_attributes = hello_child_get_default_attributes;
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
        (GInterfaceInitFunc) hello_child_atk_text_interface_init,
        (GInterfaceFinalizeFunc) NULL,
        NULL
      };

      type = g_type_register_static(atk_object_get_type(),
                                    "HelloChild", &tinfo, (GTypeFlags) 0);
      g_type_add_interface_static (type, ATK_TYPE_TEXT, &atk_text_info);
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

