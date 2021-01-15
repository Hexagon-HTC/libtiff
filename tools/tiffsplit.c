/*
 * Copyright (c) 1992-1997 Sam Leffler
 * Copyright (c) 1992-1997 Silicon Graphics, Inc.
 *
 * Permission to use, copy, modify, distribute, and sell this software and
 * its documentation for any purpose is hereby granted without fee, provided
 * that (i) the above copyright notices and this permission notice appear in
 * all copies of the software and related documentation, and (ii) the names of
 * Sam Leffler and Silicon Graphics may not be used in any advertising or
 * publicity relating to the software without the specific, prior written
 * permission of Sam Leffler and Silicon Graphics.
 *
 * THE SOFTWARE IS PROVIDED "AS-IS" AND WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS, IMPLIED OR OTHERWISE, INCLUDING WITHOUT LIMITATION, ANY
 * WARRANTY OF MERCHANTABILITY OR FITNESS FOR A PARTICULAR PURPOSE.
 *
 * IN NO EVENT SHALL SAM LEFFLER OR SILICON GRAPHICS BE LIABLE FOR
 * ANY SPECIAL, INCIDENTAL, INDIRECT OR CONSEQUENTIAL DAMAGES OF ANY KIND,
 * OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS,
 * WHETHER OR NOT ADVISED OF THE POSSIBILITY OF DAMAGE, AND ON ANY THEORY OF
 * LIABILITY, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE
 * OF THIS SOFTWARE.
 */

#include "tif_config.h"
#include "libport.h"

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "tiffio.h"

#ifndef EXIT_SUCCESS
#define EXIT_SUCCESS 0
#endif
#ifndef EXIT_FAILURE
#define EXIT_FAILURE 1
#endif

#define	CopyField(tag, v) \
    if (TIFFGetField(in, tag, &v)) TIFFSetField(out, tag, v)
#define	CopyField2(tag, v1, v2) \
    if (TIFFGetField(in, tag, &v1, &v2)) TIFFSetField(out, tag, v1, v2)
#define	CopyField3(tag, v1, v2, v3) \
    if (TIFFGetField(in, tag, &v1, &v2, &v3)) TIFFSetField(out, tag, v1, v2, v3)

static const char TIFF_SUFFIX[] = ".tif";

#define PATH_LENGTH 8192
static char fname[PATH_LENGTH] = "";

static void
newfilename(long fnum)
{
	static long lastTurn = 0;
	static short defname;
	static char *fpnt;

	if (fnum == 0) {
		if (fname[0]) {
			fpnt = fname + strlen(fname);
			defname = 0;
		} else {
			fname[0] = 'x';
			fpnt = fname + 1;
			defname = 1;
		}
	}
	const long MAXFILES = 17576;
	if (fnum == MAXFILES) {
		if (!defname || fname[0] == 'z') {
			fprintf(stderr, "tiffsplit: too many files.\n");
			exit(EXIT_FAILURE);
		}
		fname[0]++;
		fnum = 0;
	}
	if (fnum % 676 == 0) {
		if (fnum != 0) {
			/*
                         * advance to next letter every 676 pages
			 * condition for 'z'++ will be covered above
                         */
			fpnt[0]++;
		} else {
			/*
                         * set to 'a' if we are on the very first file
                         */
			fpnt[0] = 'a';
		}
		/*
                 * set the value of the last turning point
                 */
		lastTurn = fnum;
	}
	/*
         * start from 0 every 676 times (provided by lastTurn)
         * this keeps us within a-z boundaries
         */
	fpnt[1] = (char)((fnum - lastTurn) / 26) + 'a';
	/*
         * cycle last letter every file, from a-z, then repeat
         */
	fpnt[2] = (char)(fnum % 26) + 'a';
}

static int
cpStrips(TIFF* in, TIFF* out)
{
	tmsize_t bufsize = TIFFStripSize(in);
	unsigned char *buf = (unsigned char *)_TIFFmalloc(bufsize);

	if (buf == NULL) {
		return (0);
	}

	tstrip_t ns = TIFFNumberOfStrips(in);
	uint64 *bytecounts = NULL;

	if (!TIFFGetField(in, TIFFTAG_STRIPBYTECOUNTS, &bytecounts)) {
		fprintf(stderr, "tiffsplit: strip byte counts are missing\n");
                _TIFFfree(buf);
		return (0);
	}
	for (tstrip_t s = 0; s < ns; s++) {
		if (bytecounts[s] > (uint64)bufsize) {
			buf = (unsigned char *)_TIFFrealloc(buf, (tmsize_t)bytecounts[s]);
			if (!buf)
				return (0);
			bufsize = (tmsize_t)bytecounts[s];
		}
		if (TIFFReadRawStrip(in, s, buf, (tmsize_t)bytecounts[s]) < 0 ||
		    TIFFWriteRawStrip(out, s, buf, (tmsize_t)bytecounts[s]) < 0) {
			_TIFFfree(buf);
			return (0);
		}
	}
	_TIFFfree(buf);
	return (1);
}

static int
cpTiles(TIFF* in, TIFF* out)
{
	tmsize_t bufsize = TIFFTileSize(in);
	unsigned char *buf = (unsigned char *)_TIFFmalloc(bufsize);

	if (buf == NULL) {
		return (0);
	}

	const ttile_t nt = TIFFNumberOfTiles(in);
	uint64 *bytecounts = NULL;

	if (!TIFFGetField(in, TIFFTAG_TILEBYTECOUNTS, &bytecounts)) {
		fprintf(stderr, "tiffsplit: tile byte counts are missing\n");
                _TIFFfree(buf);
		return (0);
	}
	for (ttile_t t = 0; t < nt; t++) {
		if (bytecounts[t] > (uint64) bufsize) {
			buf = (unsigned char *)_TIFFrealloc(buf, (tmsize_t)bytecounts[t]);
			if (!buf)
				return (0);
			bufsize = (tmsize_t)bytecounts[t];
		}
		if (TIFFReadRawTile(in, t, buf, (tmsize_t)bytecounts[t]) < 0 ||
		    TIFFWriteRawTile(out, t, buf, (tmsize_t)bytecounts[t]) < 0) {
			_TIFFfree(buf);
			return (0);
		}
	}
	_TIFFfree(buf);
	return (1);
}

static int
tiffcp(TIFF* in, TIFF* out)
{
	uint32 longv;
	CopyField(TIFFTAG_SUBFILETYPE, longv);

	{
		uint32 w;
		CopyField(TIFFTAG_TILEWIDTH, w);
		CopyField(TIFFTAG_IMAGEWIDTH, w);
	}

	{
		uint32 l;
		CopyField(TIFFTAG_TILELENGTH, l);
		CopyField(TIFFTAG_IMAGELENGTH, l);
	}

	{
		uint16 bitspersample;
		CopyField(TIFFTAG_BITSPERSAMPLE, bitspersample);
	}

	{
		uint16 samplesperpixel;
		CopyField(TIFFTAG_SAMPLESPERPIXEL, samplesperpixel);
	}

	{
		uint16 compression;
		CopyField(TIFFTAG_COMPRESSION, compression);
		if (compression == COMPRESSION_JPEG) {
			uint32 count = 0;
			void *table = NULL;
			if (TIFFGetField(in, TIFFTAG_JPEGTABLES, &count, &table)
			    && count > 0 && table) {
			    TIFFSetField(out, TIFFTAG_JPEGTABLES, count, table);
			}
		}
	}

	uint16 shortv;
        CopyField(TIFFTAG_PHOTOMETRIC, shortv);
	CopyField(TIFFTAG_PREDICTOR, shortv);
	CopyField(TIFFTAG_THRESHHOLDING, shortv);
	CopyField(TIFFTAG_FILLORDER, shortv);
	CopyField(TIFFTAG_ORIENTATION, shortv);
	CopyField(TIFFTAG_MINSAMPLEVALUE, shortv);
	CopyField(TIFFTAG_MAXSAMPLEVALUE, shortv);

	float floatv;
	CopyField(TIFFTAG_XRESOLUTION, floatv);
	CopyField(TIFFTAG_YRESOLUTION, floatv);
	CopyField(TIFFTAG_GROUP3OPTIONS, longv);
	CopyField(TIFFTAG_GROUP4OPTIONS, longv);
	CopyField(TIFFTAG_RESOLUTIONUNIT, shortv);
	CopyField(TIFFTAG_PLANARCONFIG, shortv);
	CopyField(TIFFTAG_ROWSPERSTRIP, longv);
	CopyField(TIFFTAG_XPOSITION, floatv);
	CopyField(TIFFTAG_YPOSITION, floatv);
	CopyField(TIFFTAG_IMAGEDEPTH, longv);
	CopyField(TIFFTAG_TILEDEPTH, longv);
	CopyField(TIFFTAG_SAMPLEFORMAT, shortv);
	{
		uint16 *shortav;
		CopyField2(TIFFTAG_EXTRASAMPLES, shortv, shortav);
	}
	{
		uint16 *red, *green, *blue;
		CopyField3(TIFFTAG_COLORMAP, red, green, blue);
	}
	{
		uint16 shortv2;
		CopyField2(TIFFTAG_PAGENUMBER, shortv, shortv2);
	}

	char *stringv;
	CopyField(TIFFTAG_ARTIST, stringv);
	CopyField(TIFFTAG_IMAGEDESCRIPTION, stringv);
	CopyField(TIFFTAG_MAKE, stringv);
	CopyField(TIFFTAG_MODEL, stringv);
	CopyField(TIFFTAG_SOFTWARE, stringv);
	CopyField(TIFFTAG_DATETIME, stringv);
	CopyField(TIFFTAG_HOSTCOMPUTER, stringv);
	CopyField(TIFFTAG_PAGENAME, stringv);
	CopyField(TIFFTAG_DOCUMENTNAME, stringv);
	CopyField(TIFFTAG_BADFAXLINES, longv);
	CopyField(TIFFTAG_CLEANFAXDATA, longv);
	CopyField(TIFFTAG_CONSECUTIVEBADFAXLINES, longv);
	CopyField(TIFFTAG_FAXRECVPARAMS, longv);
	CopyField(TIFFTAG_FAXRECVTIME, longv);
	CopyField(TIFFTAG_FAXSUBADDRESS, stringv);
	CopyField(TIFFTAG_FAXDCS, stringv);
	if (TIFFIsTiled(in))
		return (cpTiles(in, out));
	else
		return (cpStrips(in, out));
}

int
main(int argc, char* argv[])
{
	if (argc < 2) {
                fprintf(stderr, "%s\n\n", TIFFGetVersion());
		fprintf(stderr, "usage: tiffsplit input.tif [prefix]\n");
		return (EXIT_FAILURE);
	}
	if (argc > 2) {
		strncpy(fname, argv[2], sizeof(fname));
		fname[sizeof(fname) - 1] = '\0';
	}

	TIFF *in = TIFFOpen(argv[1], "r");
	if (in == NULL) {
		return (EXIT_FAILURE);
	}

	long fnum = 0;
	do {
		newfilename(fnum);
		fnum++;

		const size_t path_len = strlen(fname) + sizeof(TIFF_SUFFIX);
		char *path = (char *) _TIFFmalloc(path_len);
		strncpy(path, fname, path_len);
		path[path_len - 1] = '\0';
		strncat(path, TIFF_SUFFIX, path_len - strlen(path) - 1);
		TIFF *out = TIFFOpen(path, TIFFIsBigEndian(in)?"wb":"wl");
		_TIFFfree(path);

		if (out == NULL)
			return (EXIT_FAILURE);
		if (!tiffcp(in, out))
			return (EXIT_FAILURE);
		TIFFClose(out);
	} while (TIFFReadDirectory(in));

	TIFFClose(in);

	return (EXIT_SUCCESS);
}

/* vim: set ts=8 sts=8 sw=8 noet: */
/*
 * Local Variables:
 * mode: c
 * c-basic-offset: 8
 * fill-column: 78
 * End:
 */
