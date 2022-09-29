// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#include <algorithm>
#include <iostream>
#include <sstream>

#include "contrib/stream/tiffstream.h"
#include "libtiff/tiffiop.h"

constexpr size_t kMaxMalloc = 250000000; // 1/4 GB

void *getBuffer(tmsize_t bufSize) {
  if (bufSize > kMaxMalloc)
    return nullptr;
  return _TIFFmalloc(bufSize);
}

void TIFFReadTileData(TIFF *tif, uint16 config) {

  unsigned char *buf = (unsigned char *)getBuffer(TIFFTileSize(tif));
  if (buf == nullptr)
    return;

  uint32 tw = 0, th = 0, w = 0, h = 0;
  tsample_t samplesperpixel = 1;
  TIFFGetField(tif, TIFFTAG_IMAGEWIDTH, &w);
  TIFFGetField(tif, TIFFTAG_IMAGELENGTH, &h);
  TIFFGetField(tif, TIFFTAG_TILEWIDTH, &tw);
  TIFFGetField(tif, TIFFTAG_TILELENGTH, &th);

  if (config == PLANARCONFIG_CONTIG) {
    TIFFGetField(tif, TIFFTAG_SAMPLESPERPIXEL, &samplesperpixel);
  }

  for (uint32 row = 0; row < h; row += th) {
    for (uint32 col = 0; col < w; col += tw) {
      for (tsample_t s = 0; s < samplesperpixel; s++) {
        if (TIFFReadTile(tif, buf, col, row, 0, s) < 0) {
          goto exit;
        }
      }
    }
  }

exit:
  _TIFFfree(buf);
}

void TIFFReadStripData(TIFF *tif, uint16 config) {

  unsigned char *buf = (unsigned char *)getBuffer(TIFFStripSize(tif));
  if (buf == nullptr)
    return;

  uint32 h = 0;
  uint32 rowsperstrip = (uint32)-1;
  tsample_t samplesperpixel = 1;

  tsize_t scanline = TIFFScanlineSize(tif);
  TIFFGetField(tif, TIFFTAG_IMAGELENGTH, &h);
  TIFFGetField(tif, TIFFTAG_ROWSPERSTRIP, &rowsperstrip);

  if (config == PLANARCONFIG_CONTIG) {
    TIFFGetField(tif, TIFFTAG_SAMPLESPERPIXEL, &samplesperpixel);
  }

  for (uint32 row = 0; row < h; row += rowsperstrip) {
    for (tsample_t s = 0; s < samplesperpixel; s++) {
      uint32 nrow = std::min(h - row, rowsperstrip);
      tstrip_t strip = TIFFComputeStrip(tif, row, s);
      if (TIFFReadEncodedStrip(tif, strip, buf, nrow * scanline) < 0) {
        goto exit;
      }
    }
  }

exit:
  _TIFFfree(buf);
}

void TIFFReadRawData(TIFF *tif) {

  const tstrip_t numStrips = TIFFNumberOfStrips(tif);

  uint64 *stripbc = nullptr;
  TIFFGetField(tif, TIFFTAG_STRIPBYTECOUNTS, &stripbc);

  if (numStrips < 1)
    return;

  uint32 bufSize = (uint32)stripbc[0];
  tdata_t buf = (tdata_t)getBuffer(bufSize);

  for (tstrip_t s = 0; s < numStrips; s++) {

    if (stripbc[s] > bufSize) {
      buf = _TIFFrealloc(buf, (tmsize_t)stripbc[s]);
      bufSize = (uint32)stripbc[s];
    }

    if (buf == nullptr)
      return;

    if (TIFFReadRawStrip(tif, s, buf, (tmsize_t)stripbc[s]) < 0)
      break;

    TIFFReverseBits(reinterpret_cast<uint8 *>(buf), (tmsize_t)stripbc[s]);
  }
  
  _TIFFfree(buf);
}

void TIFFReadData(TIFF *tif) {

  TIFFReadRawData(tif);

  uint16 config = PLANARCONFIG_CONTIG;
  TIFFGetField(tif, TIFFTAG_PLANARCONFIG, &config);

  if (TIFFIsTiled(tif)) {
    TIFFReadTileData(tif, config);
  } else {
    TIFFReadStripData(tif, config);
  }
}

void FuzzErrorHandler(const char *, const char *, va_list) {}
void FuzzErrorHandlerExt(thandle_t, const char *, const char *, va_list) {}

extern "C" int LLVMFuzzerTestOneInput(const uint8_t *data, size_t size) {
  TIFFSetErrorHandler(FuzzErrorHandler);
  TIFFSetErrorHandlerExt(FuzzErrorHandlerExt);
  TIFFSetWarningHandler(FuzzErrorHandler);
  TIFFSetWarningHandlerExt(FuzzErrorHandlerExt);

  const std::string s(reinterpret_cast<const char *>(data), size);
  std::istringstream iss(s);
  std::istream &stream = iss;

  TiffStream ts;
  TIFF *tif = ts.makeFileStream(&stream);
  if (tif == nullptr)
    return 0;

  toff_t exif_offset;
  do {
    exif_offset = 0;
    FILE *f = fopen("/dev/null", "w");
    TIFFPrintDirectory(tif, f, 0);

    TIFFReadData(tif);
    if (TIFFGetField(tif, TIFFTAG_EXIFIFD, &exif_offset)) {
      TIFFReadEXIFDirectory(tif, exif_offset);
    }
  } while (TIFFReadDirectory(tif));

  TIFFClose(tif);
  return 0;
}
