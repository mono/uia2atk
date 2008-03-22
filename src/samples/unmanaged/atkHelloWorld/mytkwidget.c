#include "mytkwidget.h"
#include "atk/atk.h"

static void
gtk_widget_class_init (MytkWidgetClass *klass);

GType
mytk_widget_get_type (void);

/* --- variables --- */
static gpointer         mytk_widget_parent_class = NULL;

static void
mytk_widget_finalize (GObject *object)
{
  MytkWidget *widget = MYTK_WIDGET (object);
  g_free (widget->name);
  
  //FIXME: unref the accessible component
  //MytkAccessible *accessible;
  //accessible = g_object_get_qdata (G_OBJECT (widget), quark_accessible_object);
  //if (accessible)
  //  g_object_unref (accessible);

  G_OBJECT_CLASS (mytk_widget_parent_class)->finalize (object);
}

static void
mytk_widget_base_class_finalize (MytkWidgetClass *klass)
{
  //unref props
}

static void
mytk_widget_dispose (GObject *object);

static AtkObject* 
mytk_widget_real_get_accessible (MytkWidget *widget)
{
  AtkObject* accessible = widget->accessible;

  if (!accessible)
  {
    widget->accessible = accessible = hello_child_new(widget->name);
    
//    AtkObjectFactory *factory;
//    AtkRegistry *default_registry;
//
//    default_registry = atk_get_default_registry ();
//    factory = atk_registry_get_factory (default_registry, 
//                                        G_TYPE_FROM_INSTANCE (widget));
//    accessible =
//      atk_object_factory_create_accessible (factory,
//                                            G_OBJECT (widget));
//    widget->accessible = accessible;
  }
  return accessible;
}


static void
mytk_widget_class_init (MytkWidgetClass *klass)
{
  GObjectClass *gobject_class = G_OBJECT_CLASS (klass);

  mytk_widget_parent_class = g_type_class_peek_parent (klass);

  gobject_class->dispose = mytk_widget_dispose;
  gobject_class->finalize = mytk_widget_finalize;
  //gobject_class->set_property = mytk_widget_set_property;
  //gobject_class->get_property = mytk_widget_get_property;

  //object_class->destroy = mytk_widget_real_destroy;
  
  /* Accessibility support */
  klass->get_accessible = mytk_widget_real_get_accessible;
}

static void
mytk_widget_object_init (MytkWidget *widget)
{
  widget->some_property = 2;
  widget->name = NULL;
  //widget->window = NULL;
  widget->parent_widget = NULL;
}


static void
mytk_widget_dispose (GObject *object)
{
  MytkWidget *widget = MYTK_WIDGET (object);
  G_OBJECT_CLASS (mytk_widget_parent_class)->dispose (object);
}

static AtkObject*
mytk_widget_ref_accessible (AtkImplementor *implementor);


/*
 * Initialize a AtkImplementorIface instance's virtual pointers as
 * appropriate to this implementor's class (GtkWidget).
 */
AtkObject* 
mytk_widget_get_accessible (MytkWidget *widget)
{
  MytkWidgetClass *klass;

  klass = MYTK_WIDGET_GET_CLASS (widget);

  g_return_val_if_fail (klass->get_accessible != NULL, NULL);

  return klass->get_accessible (widget);
}

static AtkObject*
mytk_widget_ref_accessible (AtkImplementor *implementor)
{
  AtkObject *accessible;

  accessible = mytk_widget_get_accessible (MYTK_WIDGET (implementor));
  if (accessible)
    g_object_ref (accessible);
  return accessible;
}

static void
mytk_widget_accessible_interface_init (AtkImplementorIface *iface)
{
  iface->ref_accessible = mytk_widget_ref_accessible;
}


GType
mytk_widget_get_type (void)
{
  static GType widget_type = 0;

  if (G_UNLIKELY (widget_type == 0))
    {
      const GTypeInfo widget_info =
      {
	sizeof (MytkWidgetClass),
	mytk_widget_object_init,		// base_init
	(GBaseFinalizeFunc) mytk_widget_base_class_finalize,
	(GClassInitFunc) mytk_widget_class_init,
	NULL,		// class_finalize
	NULL,		// class_init
	sizeof (MytkWidget),
	0,		// n_preallocs
	(GInstanceInitFunc) mytk_widget_object_init,
	NULL,		// value_table
      };

      const GInterfaceInfo accessibility_info =
      {
	(GInterfaceInitFunc) mytk_widget_accessible_interface_init,
	(GInterfaceFinalizeFunc) NULL,
	NULL // interface data 
      };

      widget_type = g_type_register_static (G_TYPE_OBJECT, "MytkWidget",
                                           &widget_info, 0);

      g_type_add_interface_static (widget_type, ATK_TYPE_IMPLEMENTOR,
                                   &accessibility_info) ;

    }

  return widget_type;
}


MytkWidget*
mytk_widget_new (
                //this seems to be used in Gtk because there Widget is an abstract class:
                //GType        type,
                const gchar *name,
                //interesting bit: similar to args keyword in C#:
                ...
                )
{
  MytkWidget *widget;
  
  //this seems to be used in Gtk because there Widget is an abstract class:
  //g_return_val_if_fail (g_type_is_a (type, MYTK_TYPE_WIDGET), NULL);
  
  //for handling the additional args --disabled
  //va_list var_args;
  //va_start (var_args, first_property_name);
  //widget = (GtkWidget *)g_object_new_valist (type, first_property_name, var_args);
  //va_end (var_args);

  widget = (MytkWidget *)g_object_new(MYTK_TYPE_WIDGET, NULL);
  widget->name = g_strdup(name);

  return widget;
}


MytkWidget*
mytk_wiget_new (gchar* name)
{
  MytkWidget* widget = g_object_new(MYTK_TYPE_WIDGET, NULL);
  widget->name = strdup(name);
}