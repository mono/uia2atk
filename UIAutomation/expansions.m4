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

AC_DEFUN([_SHAMROCK_CHECK_MONO_GAC_ASSEMBLIES],
[
	for asm in $(echo "$*" | cut -d, -f2- | sed 's/\,/ /g')
	do
		AC_MSG_CHECKING([for Mono $1 GAC for $asm.dll])
		if test \
			-e "$($PKG_CONFIG --variable=libdir mono)/mono/$1/$asm.dll" -o \
			-e "$($PKG_CONFIG --variable=prefix mono)/lib/mono/$1/$asm.dll"; \
			then \
			AC_MSG_RESULT([found])
		else
			AC_MSG_RESULT([not found])
			AC_MSG_ERROR([missing reqired Mono $1 assembly: $asm.dll])
		fi
	done
])

AC_DEFUN([SHAMROCK_CHECK_MONO_1_0_GAC_ASSEMBLIES],
[
	_SHAMROCK_CHECK_MONO_GAC_ASSEMBLIES(1.0, $*)
])

AC_DEFUN([SHAMROCK_CHECK_MONO_2_0_GAC_ASSEMBLIES],
[
	_SHAMROCK_CHECK_MONO_GAC_ASSEMBLIES(2.0, $*)
])
