#include <atk/atk.h>

#define HELLO_TYPE_CHILD                  (hello_toplevel_get_type ())
#define HELLO_CHILD(obj)               (G_TYPE_CHECK_INSTANCE_CAST ((obj), HELLO_TYPE_CHILD, HelloChild))
#define HELLO_CHILD_CLASS(klass)       (G_TYPE_CHECK_CLASS_CAST ((klass), HELLO_TYPE_CHILD, HelloChildClass))
#define HELLO_IS_CHILD(obj)            (G_TYPE_CHECK_INSTANCE_TYPE ((obj), HELLO_TYPE_CHILD))
#define HELLO_IS_CHILD_CLASS(klass)    (G_TYPE_CHECK_CLASS_TYPE ((klass), HELLO_TYPE_CHILD))
#define HELLO_CHILD_GET_CLASS(obj)     (G_TYPE_INSTANCE_GET_CLASS ((obj), HELLO_TYPE_CHILD, HelloChildClass))


typedef struct _HelloChild             HelloChild;
typedef struct _HelloChildClass             HelloChildClass;


struct _HelloChild
{
  AtkObject parent;
  GList *window_list;
};

GType hello_child_get_type (void);

struct _HelloChildClass
{
  AtkObjectClass parent_class;
};

AtkObject *hello_child_new(gchar* name);
