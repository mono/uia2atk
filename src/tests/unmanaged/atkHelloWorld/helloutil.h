
#include <atk/atk.h>

#define HELLO_TYPE_UTIL               (hello_util_get_type ())
#define HELLO_UTIL(obj)               (G_TYPE_CHECK_INSTANCE_CAST ((obj), HELLO_TYPE_UTIL, HelloUtil))
#define HELLO_UTIL_CLASS(klass)       (G_TYPE_CHECK_CLASS_CAST ((klass), HELLO_TYPE_UTIL, HelloUtilClass))
#define HELLO_IS_UTIL(obj)            (G_TYPE_CHECK_INSTANCE_TYPE ((obj), HELLO_TYPE_UTIL))
#define HELLO_IS_UTIL_CLASS(klass)    (G_TYPE_CHECK_CLASS_TYPE ((klass), HELLO_TYPE_UTIL))
#define HELLO_UTIL_GET_CLASS(obj)     (G_TYPE_INSTANCE_GET_CLASS ((obj), HELLO_TYPE_UTIL, HelloUtilClass))

typedef struct _HelloUtil                  HelloUtil;
typedef struct _HelloUtilClass             HelloUtilClass;
  
struct _HelloUtil
{
  AtkUtil parent;
  GList *listener_list;
};

GType hello_util_get_type (void);

struct _HelloUtilClass
{
  AtkUtilClass parent_class;
};

#define HELLO_TYPE_MISC                           (hello_misc_get_type ())
#define HELLO_MISC(obj)                           (G_TYPE_CHECK_INSTANCE_CAST ((obj), HELLO_TYPE_MISC, HelloMisc))
#define HELLO_MISC_CLASS(klass)                   (G_TYPE_CHECK_CLASS_CAST ((klass), HELLO_TYPE_MISC, HelloMiscClass))
#define HELLO_IS_MISC(obj)                        (G_TYPE_CHECK_INSTANCE_TYPE ((obj), HELLO_TYPE_MISC))
#define HELLO_IS_MISC_CLASS(klass)                (G_TYPE_CHECK_CLASS_TYPE ((klass), HELLO_TYPE_MISC))
#define HELLO_MISC_GET_CLASS(obj)                 (G_TYPE_INSTANCE_GET_CLASS ((obj), HELLO_TYPE_MISC, HelloMiscClass))

typedef struct _HelloMisc                  HelloMisc;
typedef struct _HelloMiscClass             HelloMiscClass;
  
struct _HelloMisc
{
  AtkMisc parent;
};

GType hello_misc_get_type (void);

struct _HelloMiscClass
{
  AtkMiscClass parent_class;
};
