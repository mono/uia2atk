#ifndef TEST_HELLO_H
#include "gtk/gtk.h"
#include <atk/atk.h>

G_BEGIN_DECLS

#define TEST_TYPE_HELLO      (test_hello_get_type ())
#define TEST_HELLO(obj)   (GTK_CHECK_CAST((obj), TEST_TYPE_HELLO, test_hello))
#define IS_TEST_HELLO(obj)     (GTK_CHECK_TYPE((obj), TEST_TYPE_HELLO))
#define IS_TEST_HELLO_CLASS(k) (GTK_CHECK_CLASS_TYPE ((k), TEST_TYPE_HELLO))
#define TEST_HELLO_CLASS(k) (GTK_CHECK_CLASS_CAST ((k), TEST_TYPE_HELLO, AtkObjectClass))

  /* forward declarations */
typedef struct _test_helloClass test_helloClass;

typedef struct test_hello test_hello;
struct test_hello
{
    AtkObject parent;
};

struct _test_helloClass
{
    AtkObjectClass parent_class;
};

GType test_hello_get_type();

G_END_DECLS
#endif /* TEST_HELLO_H */
