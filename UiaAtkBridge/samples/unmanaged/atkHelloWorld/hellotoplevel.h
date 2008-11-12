#include "mytkwidget.h"
#include <atk/atk.h>


#define HELLO_TYPE_TOPLEVEL               (hello_toplevel_get_type ())
#define HELLO_TOPLEVEL(obj)               (G_TYPE_CHECK_INSTANCE_CAST ((obj), HELLO_TYPE_TOPLEVEL, HelloToplevel))
#define HELLO_TOPLEVEL_CLASS(klass)       (G_TYPE_CHECK_CLASS_CAST ((klass), HELLO_TYPE_TOPLEVEL, HelloToplevelClass))
#define HELLO_IS_TOPLEVEL(obj)            (G_TYPE_CHECK_INSTANCE_TYPE ((obj), HELLO_TYPE_TOPLEVEL))
#define HELLO_IS_TOPLEVEL_CLASS(klass)    (G_TYPE_CHECK_CLASS_TYPE ((klass), HELLO_TYPE_TOPLEVEL))
#define HELLO_TOPLEVEL_GET_CLASS(obj)     (G_TYPE_INSTANCE_GET_CLASS ((obj), HELLO_TYPE_TOPLEVEL, HelloToplevelClass))


typedef struct _HelloToplevel             HelloToplevel;
typedef struct _HelloToplevelClass        HelloToplevelClass;


struct _HelloToplevel
{
  AtkObject parent;
  GList *window_list;
};

GType hello_toplevel_get_type (void);

struct _HelloToplevelClass
{
  AtkObjectClass parent_class;
};

AtkObject *hello_toplevel_new(void);

HelloToplevel *get_hello_toplevel_singleton(void);

void hello_toplevel_window_destroyed (MytkWidget *window);
