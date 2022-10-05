# CMake build for libtiff
#
# Copyright © 2015 Open Microscopy Environment / University of Dundee
# Written by Roger Leigh <rleigh@codelibre.net>
#
# Permission to use, copy, modify, distribute, and sell this software and
# its documentation for any purpose is hereby granted without fee, provided
# that (i) the above copyright notices and this permission notice appear in
# all copies of the software and related documentation, and (ii) the names of
# Sam Leffler and Silicon Graphics may not be used in any advertising or
# publicity relating to the software without the specific, prior written
# permission of Sam Leffler and Silicon Graphics.
#
# THE SOFTWARE IS PROVIDED "AS-IS" AND WITHOUT WARRANTY OF ANY KIND,
# EXPRESS, IMPLIED OR OTHERWISE, INCLUDING WITHOUT LIMITATION, ANY
# WARRANTY OF MERCHANTABILITY OR FITNESS FOR A PARTICULAR PURPOSE.
#
# IN NO EVENT SHALL SAM LEFFLER OR SILICON GRAPHICS BE LIABLE FOR
# ANY SPECIAL, INCIDENTAL, INDIRECT OR CONSEQUENTIAL DAMAGES OF ANY KIND,
# OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS,
# WHETHER OR NOT ADVISED OF THE POSSIBILITY OF DAMAGE, AND ON ANY THEORY OF
# LIABILITY, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE
# OF THIS SOFTWARE.

# Generate headers
configure_file(${CMAKE_CURRENT_SOURCE_DIR}/tif_config.h.cmake.in
               ${CMAKE_CURRENT_BINARY_DIR}/tif_config.h
               @ONLY)
configure_file(${CMAKE_CURRENT_SOURCE_DIR}/tiffconf.h.cmake.in
               ${CMAKE_CURRENT_BINARY_DIR}/tiffconf.h
               @ONLY)

extra_dist(
  SConstruct
  tif_config.h-vms
  tif_config.vc.h
  tif_config.wince.h
  tiffconf.vc.h
  tiffconf.wince.h
  libtiff.def
  libtiff.map
  libtiffxx.map)

set(tiff_HEADERS
  tiff.h
  tiffio.h
  tiffvers.h)

set(tiff_noinst_HEADERS
  t4.h
  tif_dir.h
  tif_predict.h
  tiffiop.h
  uvcode.h)

set(nodist_tiff_HEADERS
  ${CMAKE_CURRENT_BINARY_DIR}/tiffconf.h)

set(tiff_SOURCES
  tif_aux.c
  tif_close.c
  tif_codec.c
  tif_color.c
  tif_compress.c
  tif_dir.c
  tif_dirinfo.c
  tif_dirread.c
  tif_dirwrite.c
  tif_dumpmode.c
  tif_error.c
  tif_extension.c
  tif_fax3.c
  tif_fax3sm.c
  tif_flush.c
  tif_getimage.c
  tif_jbig.c
  tif_jpeg.c
  tif_jpeg_12.c
  tif_luv.c
  tif_lzma.c
  tif_lzw.c
  tif_next.c
  tif_ojpeg.c
  tif_open.c
  tif_packbits.c
  tif_pixarlog.c
  tif_predict.c
  tif_print.c
  tif_read.c
  tif_strip.c
  tif_swab.c
  tif_thunder.c
  tif_tile.c
  tif_version.c
  tif_warning.c
  tif_webp.c
  tif_write.c
  tif_zip.c
  tif_zstd.c)

set(tiffxx_HEADERS
  tiffio.hxx)

set(tiffxx_SOURCES
  tif_stream.cxx)

if(USE_WIN32_FILEIO)
  extra_dist(tif_unix.c)
  list(APPEND tiff_SOURCES tif_win32.c)
else()
  extra_dist(tif_win32.c)
  list(APPEND tiff_SOURCES tif_unix.c)
endif()

add_library(TIFF ${tiff_SOURCES} ${tiff_HEADERS} ${nodist_tiff_HEADERS}
            ${tiff_port_SOURCES} libtiff.def)

set_target_properties(TIFF PROPERTIES
    PDB_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}
    COMPILE_PDB_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}
    PDB_NAME tiff
    COMPILE_PDB_NAME tiff
)

target_compile_definitions(TIFF PUBLIC $<$<CONFIG:Debug>:TIFF_DEBUG>)
target_include_directories(TIFF
    PUBLIC
        $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}>
        $<BUILD_INTERFACE:${CMAKE_CURRENT_BINARY_DIR}>
        $<INSTALL_INTERFACE:${CMAKE_INSTALL_INCLUDEDIR}>
        ${TIFF_INCLUDES}
)
target_link_libraries(TIFF ${TIFF_LIBRARY_DEPS})
if(NOT CYGWIN)
    # This property causes shared libraries on Linux to have the full version
    # encoded into their final filename.  We disable this on Cygwin because
    # it causes cygz-${TIFF_FULL_VERSION}.dll to be created when cygz.dll
    # seems to be the default.
    set_target_properties(TIFF PROPERTIES VERSION ${SO_VERSION})
endif()
if(HAVE_LD_VERSION_SCRIPT)
  set_target_properties(TIFF PROPERTIES LINK_FLAGS
                        "-Wl,--version-script=${CMAKE_CURRENT_SOURCE_DIR}/libtiff.map")
endif()

install(TARGETS TIFF EXPORT tiff
        RUNTIME DESTINATION ${CMAKE_INSTALL_BINDIR}
        LIBRARY DESTINATION ${CMAKE_INSTALL_LIBDIR}
        ARCHIVE DESTINATION ${CMAKE_INSTALL_LIBDIR})

install(FILES ${tiff_HEADERS} ${nodist_tiff_HEADERS}
        DESTINATION "${CMAKE_INSTALL_FULL_INCLUDEDIR}")

if(CXX_SUPPORT)
  add_library(tiffxx ${tiffxx_SOURCES} ${tiffxx_HEADERS})
  target_link_libraries(tiffxx TIFF)
  if(NOT CYGWIN)
    # This property causes shared libraries on Linux to have the full version
    # encoded into their final filename.  We disable this on Cygwin because
    # it causes cygz-${TIFF_FULL_VERSION}.dll to be created when cygz.dll
    # seems to be the default.
    set_target_properties(tiffxx PROPERTIES VERSION ${SO_VERSION})
  endif()
  if(HAVE_LD_VERSION_SCRIPT)
    set_target_properties(tiffxx PROPERTIES LINK_FLAGS
                          "-Wl,--version-script=${CMAKE_CURRENT_SOURCE_DIR}/libtiffxx.map")
  endif()

  install(TARGETS tiffxx
          RUNTIME DESTINATION ${CMAKE_INSTALL_BINDIR}
          LIBRARY DESTINATION ${CMAKE_INSTALL_LIBDIR}
          ARCHIVE DESTINATION ${CMAKE_INSTALL_LIBDIR})

  install(FILES ${tiffxx_HEADERS}
          DESTINATION "${CMAKE_INSTALL_FULL_INCLUDEDIR}")

endif()

export(PACKAGE tiff)
include(CMakePackageConfigHelpers)
configure_package_config_file(
  "tiffConfig.cmake.in"
  "${CMAKE_CURRENT_BINARY_DIR}/bin/tiff-config.cmake"
  INSTALL_DESTINATION "${CMAKE_INSTALL_LIBDIR}/cmake/tiff"
)
write_basic_package_version_file(
  "${CMAKE_CURRENT_BINARY_DIR}/bin/tiffConfigVersion.cmake"
  VERSION "${LIBTIFF_MAJOR_VERSION}.${LIBTIFF_MINOR_VERSION}.${LIBTIFF_MICRO_VERSION}${LIBTIFF_ALPHA_VERSION}"
  COMPATIBILITY SameMajorVersion
)

install(FILES
        ${CMAKE_CURRENT_BINARY_DIR}/bin/tiff-config.cmake
        ${CMAKE_CURRENT_BINARY_DIR}/bin/tiffConfigVersion.cmake
        DESTINATION ${CMAKE_INSTALL_LIBDIR}/cmake/tiff)

install(EXPORT tiff NAMESPACE TIFF:: DESTINATION ${CMAKE_INSTALL_LIBDIR}/cmake/tiff FILE tiffTargets.cmake)

if(MSVC)
    install(FILES ${CMAKE_BINARY_DIR}/Debug/tiff.pdb DESTINATION $<IF:$<BOOL:${BUILD_SHARED_LIBS}>,${CMAKE_INSTALL_BINDIR},${CMAKE_INSTALL_LIBDIR}> CONFIGURATIONS Debug)
endif(MSVC)