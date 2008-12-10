# vim: set ts=4 sw=4 expandtab et: coding=UTF-8
#!/usr/bin/env python


try:
    import xml.etree.ElementTree as ET
except ImportError:
    try:
        import cElementTree as ET # cElementTree is faster
    except ImportError:
        import elementtree.ElementTree as ET # fallback on regular ElementTree

class XMLParser(object):

    config = None
    def __init__(self, conf):
        self.config = conf
    
    def get_spec_file(self, module):
        return self._module_parser(module, "spec_file").text
    
    def get_version_file(self, module):
        return self._module_parser(module, "version_file").text

    def get_version_update(self, module):
        return self._module_parser(module, "version_update").text

    def get_dependencies(self, module):
        '''Returns a list'''
        deps = self._module_parser(module, "dependencies")
        deps_list = []
        for d in deps:
            deps_list.append(d.text)
        return tuple(deps_list)

    def get_patches(self, module):
        '''Returns a list'''
        pats = self._module_parser(module, "patches")
        pats_list = []
        for p in pats:
            pats_list.append(p.text)
        return tuple(pats_list)

    def _module_parser(self, module, element):
        tree = ET.ElementTree()
        tree.parse(self.config)
        modules = tree.find("modules")
        for m in modules:
            if m.tag == module:
                r = m.find(element)
                return r 
        
#if __name__ == "__main__":
#    
#    x = XMLParser("uia.xml")
#    print "SPEC FILE\t"
#    print x.get_spec_file("gtk-sharp212")
#    print "VERSION FILE\t"
#    print x.get_version_file("gtk-sharp212")
#    print "VERSION UPDATE STRING\t"
#    print x.get_version_update("gtk-sharp212")
#    deps = x.get_dependencies("gtk-sharp212")
#    print "DEPENDENCIES\t"
#    print "\n".join(deps)
#    pats = x.get_patches("gtk-sharp212")
#    print "PATCHES\t"
#    print "\n".join(pats)
