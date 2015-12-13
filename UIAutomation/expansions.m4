AC_DEFUN([SHAMROCK_EXPAND_LIBDIR],
[	
	expanded_libdir=`(
		case $prefix in 
			NONE) prefix=$ac_default_prefix ;; 
			*) ;; 
		esac
		case $exec_prefix in 
			NONE) exec_prefix=$prefix ;; 
			*) ;; 
		esac
		eval echo $libdir
	)`
	AC_SUBST(expanded_libdir)
])

AC_DEFUN([SHAMROCK_EXPAND_BINDIR],
[
	expanded_bindir=`(
		case $prefix in 
			NONE) prefix=$ac_default_prefix ;; 
			*) ;; 
		esac
		case $exec_prefix in 
			NONE) exec_prefix=$prefix ;; 
			*) ;; 
		esac
		eval echo $bindir
	)`
	AC_SUBST(expanded_bindir)
])

AC_DEFUN([SHAMROCK_EXPAND_DATADIR],
[
	case $prefix in
		NONE) prefix=$ac_default_prefix ;;
		*) ;;
	esac

	case $exec_prefix in
		NONE) exec_prefix=$prefix ;;
		*) ;;
	esac

	expanded_datadir=`(eval echo $datadir)`
	expanded_datadir=`(eval echo $expanded_datadir)`

	AC_SUBST(expanded_datadir)
])


AC_DEFUN([SHAMROCK_CHECK_MONO_GAC_ASSEMBLIES],
[
	AC_MSG_CHECKING([for Mono GAC for $asm.dll])
	if test \
		-e "$($PKG_CONFIG --variable=libdir mono)/mono/2.0/$1.dll" -o \
		-e "$($PKG_CONFIG --variable=prefix mono)/lib/mono/2.0/$1.dll" -o \
		-e "$($PKG_CONFIG --variable=libdir mono)/mono/4.0/$1.dll" -o \
		-e "$($PKG_CONFIG --variable=prefix mono)/lib/mono/4.0/$1.dll" -o \
		-e "$($PKG_CONFIG --variable=libdir mono)/mono/4.5/$1.dll" -o \
		-e "$($PKG_CONFIG --variable=prefix mono)/lib/mono/4.5/$1.dll"; \
		then \
		AC_MSG_RESULT([found])
	else
		AC_MSG_RESULT([not found])
		AC_MSG_ERROR([missing required Mono assembly: $1.dll])
	fi
])
