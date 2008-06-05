// Permission is hereby granted, free of charge, to any person obtaining 
// a copy of this software and associated documentation files (the 
// "Software"), to deal in the Software without restriction, including 
// without limitation the rights to use, copy, modify, merge, publish, 
// distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to 
// the following conditions: 
//  
// The above copyright notice and this permission notice shall be 
// included in all copies or substantial portions of the Software. 
//  
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// 
// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//      Andres G. Aragoneses <aaragoneses@novell.com>
// 

//this glue library is born from the need of a workaround to bug , bringing optimization, and is based on r101690


static GHashTable *listener_list = NULL;

guint atksharp_util_add_global_event_listener(
	GSignalEmissionHook listener,
	const gchar *event_type);

static void
_listener_info_destroy (gpointer data)
{
	g_free(data);
}

void
atksharp_util_override_add_global_event_listener (gpointer cb)
{
	AtkUtilClass *klass = g_type_class_peek (ATK_TYPE_UTIL);
	if (!klass)
		klass = g_type_class_ref (ATK_TYPE_UTIL);
	//disabled until we can get rid of the workaround:
	//((AtkUtilClass *) klass)->add_global_event_listener = cb;
	((AtkUtilClass *) klass)->add_global_event_listener = atksharp_util_add_global_event_listener;
	
	listener_list = g_hash_table_new_full(g_int_hash, g_int_equal, NULL, _listener_info_destroy);
}


static gint listener_idx = 1;

typedef struct _AtkUtilListenerInfo AtkUtilListenerInfo;

struct _AtkUtilListenerInfo
{
   gint key;
   guint signal_id;
   gulong hook_id;
};


guint
_atksharp_add_listener (GSignalEmissionHook listener,
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
          AtkUtilListenerInfo *listener_info;

          rc = listener_idx;

          listener_info = g_malloc(sizeof(AtkUtilListenerInfo));
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

guint atksharp_util_add_global_event_listener(
	GSignalEmissionHook listener,
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
		rc = _atksharp_add_listener (listener, split_string[1], split_string[2], event_type);

		g_strfreev (split_string);
	}

	return rc;
}
