#include "glib.h"
#include <glib-object.h>
#include "atk/atk.h"

#define MYTK_WIDGET(widget)		  (G_TYPE_CHECK_INSTANCE_CAST ((widget), MYTK_TYPE_WIDGET, MytkWidget))
#define MYTK_TYPE_WIDGET			(mytk_widget_get_type ())
#define MYTK_WIDGET_GET_CLASS(obj)         (G_TYPE_INSTANCE_GET_CLASS ((obj), MYTK_TYPE_WIDGET, MytkWidgetClass))

typedef struct _MytkWidget             MytkWidget;
typedef struct _MytkWidgetClass        MytkWidgetClass;

struct _MytkWidget
{
  gchar *name;
  gint some_property;
  
  //MytkWindow *window;
  MytkWidget *parent;
};

struct _MytkWidgetClass
{
  /* The object class structure needs to be the first
   *  element in the widget class structure in order for
   *  the class mechanism to work correctly. This allows a
   *  GtkWidgetClass pointer to be cast to a GtkObjectClass
   *  pointer.
   */
  GObjectClass parent_class;

  /*< public >*/
  
  /* accessibility support */
  AtkObject*   (*get_accessible)     (MytkWidget *widget);

  /*< private >*/

  /* basics */
  void (* somefunc)		       (MytkWidget        *widget);
  
};

//GType mytk_widget_get_type (void);
