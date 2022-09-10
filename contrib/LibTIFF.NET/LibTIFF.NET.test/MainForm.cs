/// ###########################################################################
/// Test application for LibTIFF.NET
/// ================================
/// So far, the test program deals with 16bit grayscale images only. It allows
/// to create, open (and close) and to display a TIFF file. Clicking on the 
/// bitmap leads to re-loading the image which is then displayed using a 
/// grayscale lookup table of random width. (This might be useful to test the 
/// speed of re-contrasting an image).
/// 
/// Author: Reinhard Mayr aka Czerwinski

using LibTIFF_NET;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace test
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private IntPtr tiff = IntPtr.Zero;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbVersion;
		private System.Windows.Forms.OpenFileDialog ofdOpenTIFF;
		private System.Windows.Forms.Button btnOpenTIFF;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbTIFFPtr;
		private System.Windows.Forms.Button btnCloseTIFF;
		private System.Windows.Forms.TextBox tbRasterScanlineSize;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tbScanlineSize;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox tbImageDimensions;
		private System.Windows.Forms.TextBox tbTileDimensions;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox tbBitsPerSample;
		private System.Windows.Forms.TextBox tbStrips;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox tbRowsPerStrip;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.PictureBox pbPicture;
		private System.Windows.Forms.TextBox tbSamplesPerPixel;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox tbInitialScan0Ptr;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox tbLastScan0Ptr;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox tbContrastingTime;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Button tbCreateTIFF;
		private System.Windows.Forms.SaveFileDialog sfdCreateTIFF;
		private System.Windows.Forms.CheckBox chkSaveStrips;
		private System.Windows.Forms.TextBox tbSizeFactorY;
		private System.Windows.Forms.TextBox tbSizeFactorX;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.TextBox tbSaveRowsPerStrip;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label7;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.tbVersion.Text = LibTIFF.GetVersion();
			tbTIFFPtr.Text = tiff.ToString();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.tbVersion = new System.Windows.Forms.TextBox();
			this.ofdOpenTIFF = new System.Windows.Forms.OpenFileDialog();
			this.btnOpenTIFF = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.tbTIFFPtr = new System.Windows.Forms.TextBox();
			this.btnCloseTIFF = new System.Windows.Forms.Button();
			this.tbRasterScanlineSize = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.tbScanlineSize = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.tbImageDimensions = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.tbTileDimensions = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.tbBitsPerSample = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.tbStrips = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.tbRowsPerStrip = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.pbPicture = new System.Windows.Forms.PictureBox();
			this.tbSamplesPerPixel = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.tbInitialScan0Ptr = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.tbLastScan0Ptr = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.tbContrastingTime = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.tbCreateTIFF = new System.Windows.Forms.Button();
			this.sfdCreateTIFF = new System.Windows.Forms.SaveFileDialog();
			this.chkSaveStrips = new System.Windows.Forms.CheckBox();
			this.tbSizeFactorY = new System.Windows.Forms.TextBox();
			this.tbSizeFactorX = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.tbSaveRowsPerStrip = new System.Windows.Forms.TextBox();
			this.label17 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "libtiff version:";
			// 
			// tbVersion
			// 
			this.tbVersion.Enabled = false;
			this.tbVersion.Location = new System.Drawing.Point(88, 16);
			this.tbVersion.Multiline = true;
			this.tbVersion.Name = "tbVersion";
			this.tbVersion.Size = new System.Drawing.Size(232, 72);
			this.tbVersion.TabIndex = 1;
			this.tbVersion.Text = "";
			// 
			// ofdOpenTIFF
			// 
			this.ofdOpenTIFF.Title = "Open TIFF File";
			// 
			// btnOpenTIFF
			// 
			this.btnOpenTIFF.Location = new System.Drawing.Point(16, 112);
			this.btnOpenTIFF.Name = "btnOpenTIFF";
			this.btnOpenTIFF.Size = new System.Drawing.Size(112, 24);
			this.btnOpenTIFF.TabIndex = 2;
			this.btnOpenTIFF.Text = "&Open TIFF File ...";
			this.btnOpenTIFF.Click += new System.EventHandler(this.btnOpenTIFF_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(144, 114);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(48, 16);
			this.label2.TabIndex = 3;
			this.label2.Text = "TIFF *:";
			// 
			// tbTIFFPtr
			// 
			this.tbTIFFPtr.Enabled = false;
			this.tbTIFFPtr.Location = new System.Drawing.Point(184, 112);
			this.tbTIFFPtr.Name = "tbTIFFPtr";
			this.tbTIFFPtr.TabIndex = 4;
			this.tbTIFFPtr.Text = "";
			// 
			// btnCloseTIFF
			// 
			this.btnCloseTIFF.Location = new System.Drawing.Point(304, 112);
			this.btnCloseTIFF.Name = "btnCloseTIFF";
			this.btnCloseTIFF.Size = new System.Drawing.Size(96, 23);
			this.btnCloseTIFF.TabIndex = 5;
			this.btnCloseTIFF.Text = "&Close TIFF File";
			this.btnCloseTIFF.Click += new System.EventHandler(this.btnCloseTIFF_Click);
			// 
			// tbRasterScanlineSize
			// 
			this.tbRasterScanlineSize.Enabled = false;
			this.tbRasterScanlineSize.Location = new System.Drawing.Point(128, 142);
			this.tbRasterScanlineSize.Name = "tbRasterScanlineSize";
			this.tbRasterScanlineSize.TabIndex = 7;
			this.tbRasterScanlineSize.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 144);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(104, 16);
			this.label3.TabIndex = 6;
			this.label3.Text = "RasterScanlineSize";
			// 
			// tbScanlineSize
			// 
			this.tbScanlineSize.Enabled = false;
			this.tbScanlineSize.Location = new System.Drawing.Point(128, 168);
			this.tbScanlineSize.Name = "tbScanlineSize";
			this.tbScanlineSize.TabIndex = 9;
			this.tbScanlineSize.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(16, 170);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(96, 16);
			this.label4.TabIndex = 8;
			this.label4.Text = "ScanlineSize";
			// 
			// tbImageDimensions
			// 
			this.tbImageDimensions.Enabled = false;
			this.tbImageDimensions.Location = new System.Drawing.Point(128, 192);
			this.tbImageDimensions.Name = "tbImageDimensions";
			this.tbImageDimensions.TabIndex = 11;
			this.tbImageDimensions.Text = "";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(16, 194);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(112, 16);
			this.label5.TabIndex = 10;
			this.label5.Text = "image width x height";
			// 
			// tbTileDimensions
			// 
			this.tbTileDimensions.Enabled = false;
			this.tbTileDimensions.Location = new System.Drawing.Point(128, 216);
			this.tbTileDimensions.Name = "tbTileDimensions";
			this.tbTileDimensions.TabIndex = 13;
			this.tbTileDimensions.Text = "";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(16, 218);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(112, 16);
			this.label6.TabIndex = 12;
			this.label6.Text = "tile width x height";
			// 
			// tbBitsPerSample
			// 
			this.tbBitsPerSample.Enabled = false;
			this.tbBitsPerSample.Location = new System.Drawing.Point(352, 144);
			this.tbBitsPerSample.Name = "tbBitsPerSample";
			this.tbBitsPerSample.Size = new System.Drawing.Size(32, 20);
			this.tbBitsPerSample.TabIndex = 15;
			this.tbBitsPerSample.Text = "";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(256, 146);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(104, 16);
			this.label7.TabIndex = 14;
			this.label7.Text = "bits per sample";
			// 
			// tbStrips
			// 
			this.tbStrips.Enabled = false;
			this.tbStrips.Location = new System.Drawing.Point(352, 192);
			this.tbStrips.Name = "tbStrips";
			this.tbStrips.Size = new System.Drawing.Size(32, 20);
			this.tbStrips.TabIndex = 17;
			this.tbStrips.Text = "";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(256, 194);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(88, 16);
			this.label8.TabIndex = 16;
			this.label8.Text = "strips";
			// 
			// tbRowsPerStrip
			// 
			this.tbRowsPerStrip.Enabled = false;
			this.tbRowsPerStrip.Location = new System.Drawing.Point(352, 216);
			this.tbRowsPerStrip.Name = "tbRowsPerStrip";
			this.tbRowsPerStrip.Size = new System.Drawing.Size(32, 20);
			this.tbRowsPerStrip.TabIndex = 19;
			this.tbRowsPerStrip.Text = "";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(256, 218);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(88, 16);
			this.label9.TabIndex = 18;
			this.label9.Text = "rows per strip";
			// 
			// pbPicture
			// 
			this.pbPicture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.pbPicture.Location = new System.Drawing.Point(416, 0);
			this.pbPicture.Name = "pbPicture";
			this.pbPicture.Size = new System.Drawing.Size(400, 424);
			this.pbPicture.TabIndex = 20;
			this.pbPicture.TabStop = false;
			this.pbPicture.Click += new System.EventHandler(this.pbPicture_Click);
			// 
			// tbSamplesPerPixel
			// 
			this.tbSamplesPerPixel.Enabled = false;
			this.tbSamplesPerPixel.Location = new System.Drawing.Point(352, 168);
			this.tbSamplesPerPixel.Name = "tbSamplesPerPixel";
			this.tbSamplesPerPixel.Size = new System.Drawing.Size(32, 20);
			this.tbSamplesPerPixel.TabIndex = 22;
			this.tbSamplesPerPixel.Text = "";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(256, 170);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(104, 16);
			this.label10.TabIndex = 21;
			this.label10.Text = "samples per pixel";
			// 
			// tbInitialScan0Ptr
			// 
			this.tbInitialScan0Ptr.Enabled = false;
			this.tbInitialScan0Ptr.Location = new System.Drawing.Point(128, 256);
			this.tbInitialScan0Ptr.Name = "tbInitialScan0Ptr";
			this.tbInitialScan0Ptr.TabIndex = 24;
			this.tbInitialScan0Ptr.Text = "";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(16, 258);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(88, 16);
			this.label11.TabIndex = 23;
			this.label11.Text = "initial scan0 ptr:";
			// 
			// tbLastScan0Ptr
			// 
			this.tbLastScan0Ptr.Enabled = false;
			this.tbLastScan0Ptr.Location = new System.Drawing.Point(128, 280);
			this.tbLastScan0Ptr.Name = "tbLastScan0Ptr";
			this.tbLastScan0Ptr.TabIndex = 26;
			this.tbLastScan0Ptr.Text = "";
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(16, 280);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(88, 16);
			this.label12.TabIndex = 25;
			this.label12.Text = "last scan0 ptr:";
			// 
			// tbContrastingTime
			// 
			this.tbContrastingTime.Enabled = false;
			this.tbContrastingTime.Location = new System.Drawing.Point(336, 280);
			this.tbContrastingTime.Name = "tbContrastingTime";
			this.tbContrastingTime.Size = new System.Drawing.Size(48, 20);
			this.tbContrastingTime.TabIndex = 28;
			this.tbContrastingTime.Text = "";
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(256, 282);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(88, 16);
			this.label13.TabIndex = 27;
			this.label13.Text = "contrasting time";
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(384, 282);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(24, 16);
			this.label14.TabIndex = 29;
			this.label14.Text = "ms";
			// 
			// tbCreateTIFF
			// 
			this.tbCreateTIFF.Location = new System.Drawing.Point(16, 328);
			this.tbCreateTIFF.Name = "tbCreateTIFF";
			this.tbCreateTIFF.Size = new System.Drawing.Size(112, 24);
			this.tbCreateTIFF.TabIndex = 30;
			this.tbCreateTIFF.Text = "Create TIFF File";
			this.tbCreateTIFF.Click += new System.EventHandler(this.tbCreateTIFF_Click);
			// 
			// chkSaveStrips
			// 
			this.chkSaveStrips.Location = new System.Drawing.Point(144, 352);
			this.chkSaveStrips.Name = "chkSaveStrips";
			this.chkSaveStrips.Size = new System.Drawing.Size(80, 24);
			this.chkSaveStrips.TabIndex = 31;
			this.chkSaveStrips.Text = "save strips";
			// 
			// tbSizeFactorY
			// 
			this.tbSizeFactorY.Location = new System.Drawing.Point(272, 328);
			this.tbSizeFactorY.Name = "tbSizeFactorY";
			this.tbSizeFactorY.Size = new System.Drawing.Size(32, 20);
			this.tbSizeFactorY.TabIndex = 35;
			this.tbSizeFactorY.Text = "5";
			// 
			// tbSizeFactorX
			// 
			this.tbSizeFactorX.Location = new System.Drawing.Point(184, 328);
			this.tbSizeFactorX.Name = "tbSizeFactorX";
			this.tbSizeFactorX.Size = new System.Drawing.Size(32, 20);
			this.tbSizeFactorX.TabIndex = 33;
			this.tbSizeFactorX.Text = "5";
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(224, 328);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(48, 16);
			this.label15.TabIndex = 34;
			this.label15.Text = "x 1024 *";
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(144, 328);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(40, 16);
			this.label16.TabIndex = 32;
			this.label16.Text = "1024 *";
			// 
			// tbSaveRowsPerStrip
			// 
			this.tbSaveRowsPerStrip.Location = new System.Drawing.Point(320, 360);
			this.tbSaveRowsPerStrip.Name = "tbSaveRowsPerStrip";
			this.tbSaveRowsPerStrip.Size = new System.Drawing.Size(48, 20);
			this.tbSaveRowsPerStrip.TabIndex = 37;
			this.tbSaveRowsPerStrip.Text = "128";
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(240, 360);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(72, 16);
			this.label17.TabIndex = 36;
			this.label17.Text = "rows per strip";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(816, 422);
			this.Controls.Add(this.tbSaveRowsPerStrip);
			this.Controls.Add(this.label17);
			this.Controls.Add(this.tbSizeFactorY);
			this.Controls.Add(this.tbSizeFactorX);
			this.Controls.Add(this.label15);
			this.Controls.Add(this.label16);
			this.Controls.Add(this.chkSaveStrips);
			this.Controls.Add(this.tbCreateTIFF);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.tbContrastingTime);
			this.Controls.Add(this.tbLastScan0Ptr);
			this.Controls.Add(this.tbInitialScan0Ptr);
			this.Controls.Add(this.tbSamplesPerPixel);
			this.Controls.Add(this.tbRowsPerStrip);
			this.Controls.Add(this.tbStrips);
			this.Controls.Add(this.tbBitsPerSample);
			this.Controls.Add(this.tbTileDimensions);
			this.Controls.Add(this.tbImageDimensions);
			this.Controls.Add(this.tbScanlineSize);
			this.Controls.Add(this.tbRasterScanlineSize);
			this.Controls.Add(this.tbTIFFPtr);
			this.Controls.Add(this.tbVersion);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.pbPicture);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.btnCloseTIFF);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btnOpenTIFF);
			this.Controls.Add(this.label1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			try 
			{
				Application.Run(new Form1());
			}
			catch (Exception ex) 
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void btnOpenTIFF_Click(object sender, System.EventArgs e)
		{
			if (ofdOpenTIFF.ShowDialog() != DialogResult.OK) return;

			// open TIFF file
			string filename = ofdOpenTIFF.FileName;
			tiff = LibTIFF.Open(filename,"r");

			if (tiff == IntPtr.Zero)
			{
				MessageBox.Show("Failed to open " + filename);
				return;
			}

			// report some file info
			tbTIFFPtr.Text = tiff.ToString();
			tbRasterScanlineSize.Text = LibTIFF.RasterScanlineSize(tiff).ToString();
			tbScanlineSize.Text = LibTIFF.ScanlineSize(tiff).ToString();

			UInt32 width, length;
			if (LibTIFF.GetField(tiff, (int)TIFFTags.TIFFTAG_IMAGEWIDTH, out width) &&
				LibTIFF.GetField(tiff, (int)TIFFTags.TIFFTAG_IMAGELENGTH, out length))
			{
				tbImageDimensions.Text = width.ToString() + "x" + length.ToString();
			}
			if (LibTIFF.IsTiled(tiff))
			{
				if (LibTIFF.GetField(tiff, (int)TIFFTags.TIFFTAG_TILEWIDTH, out width) &&
					LibTIFF.GetField(tiff, (int)TIFFTags.TIFFTAG_TILELENGTH, out length))
				{
					tbTileDimensions.Text = width.ToString() + "x" + length.ToString();
				}
			}
			else
			{
				tbTileDimensions.Text = "not tiled";
				UInt32 rows;
				if (LibTIFF.GetField(tiff, (int)TIFFTags.TIFFTAG_ROWSPERSTRIP, out rows))
				{
					tbRowsPerStrip.Text = rows.ToString();
					}
				}
			UInt16 bps;
			if (LibTIFF.GetField(tiff, (int)TIFFTags.TIFFTAG_BITSPERSAMPLE, out bps))
			{
				tbBitsPerSample.Text = bps.ToString();
			}

			UInt16 spp;
			if (LibTIFF.GetField(tiff, (int)TIFFTags.TIFFTAG_SAMPLESPERPIXEL, out spp))
			{
				tbSamplesPerPixel.Text = spp.ToString();
			}

			// place TIFF content in the picture box
			pbPicture.SizeMode = PictureBoxSizeMode.StretchImage;
			pbPicture.Image = GetBitmapFromTIFFHandle(tiff,0,65535);
			System.Console.WriteLine ("System.GC.GetTotalMemory: " + System.GC.GetTotalMemory(true));
		}

		private void btnCloseTIFF_Click(object sender, System.EventArgs e)
		{
			if (tiff == IntPtr.Zero) return;

			// reset handle, text fields
			LibTIFF.Close(tiff);
			tiff = (IntPtr)0;
			tbTIFFPtr.Text = tiff.ToString();
			tbRasterScanlineSize.Text = "";
			tbScanlineSize.Text = "";
			tbImageDimensions.Text = "";
			tbTileDimensions.Text = "";
			tbBitsPerSample.Text = "";
			tbRowsPerStrip.Text = "";
			tbSamplesPerPixel.Text = "";
			tbInitialScan0Ptr.Text = "";
			tbLastScan0Ptr.Text = "";
			tbContrastingTime.Text = "";

			// free bitmat data
			BitmapData bmpData = ((Bitmap)pbPicture.Image).LockBits(
				new Rectangle(0,0,pbPicture.Image.Width,pbPicture.Image.Height),
				ImageLockMode.ReadWrite,
				PixelFormat.Format8bppIndexed);
			LibTIFF._free(bmpData.Scan0);
			((Bitmap)pbPicture.Image).UnlockBits(bmpData);
			// dispose and clear Bitmap object
			pbPicture.Image.Dispose();
			pbPicture.Image = null;
		}

		/// <summary>Creates a Bitmap object from a libtiff handle.</summary>
		/// <param name="tiff">Source image handle, 16 bit grayscale</param>
		/// <param name="min">Min source histogram value => mapped to 0</param>
		/// <param name="max">Max source histogram value => mapped to 255</param>
		/// <returns>The Bitmap object. Remember to dispose the bitmap data 
		/// before abandoning the object!</returns>
		private Bitmap GetBitmapFromTIFFHandle(IntPtr tiff, ushort min, ushort max)
		{
			UInt32 imageWidth, imageLength;
			UInt16 bitsPerSample, samplesPerPixel;
			int scanlineSize = LibTIFF.ScanlineSize(tiff);
			Bitmap bmp;

			// get image parameters
			LibTIFF.GetField(tiff, (int)TIFFTags.TIFFTAG_IMAGEWIDTH, out imageWidth);
			LibTIFF.GetField(tiff, (int)TIFFTags.TIFFTAG_IMAGELENGTH, out imageLength);
			LibTIFF.GetField(tiff, (int)TIFFTags.TIFFTAG_BITSPERSAMPLE, out bitsPerSample);
			LibTIFF.GetField(tiff, (int)TIFFTags.TIFFTAG_SAMPLESPERPIXEL, out samplesPerPixel);

			if (bitsPerSample != 16 || samplesPerPixel != 1)
			{
				// this is not a 16bit grayscale TIFF file -- return
				return null;
			}

			// allocate memory for bitmap
			IntPtr bmpData = LibTIFF._malloc((int)(scanlineSize/2 * imageLength));
			tbInitialScan0Ptr.Text = bmpData.ToString();

			// read scanline by scanline
			IntPtr pScanline = LibTIFF._malloc(scanlineSize);
			for (UInt32 row=0; row < imageLength; row++)
			{
				LibTIFF.ReadScanline(tiff, pScanline, row);
				unsafe
				{
					ushort *srcBits16bit = (ushort *) pScanline;	// source: 16bit grayscale bitmap
					byte *dstBitsPalette = (byte *) bmpData;		// dest.: 8bit palette bitmap

					for (int i=0; i<imageWidth; i++)
					{
						// scale each pixel's color according to given limits
						if (srcBits16bit[i]<=min)
						{
							dstBitsPalette[i+row*imageWidth] = 0;
						}
						else if (srcBits16bit[i]>=max)
						{
							dstBitsPalette[i+row*imageWidth] = 255;
						}
						else
						{
							dstBitsPalette[i+row*imageWidth] =
								(byte)(((long)srcBits16bit[i]-(long)min)*(long)255/((long)max-min));
						}
					}
				}
			}
			LibTIFF._free(pScanline);

			// create the Bitmap object using the read bitmap data
			bmp = new Bitmap((int)imageWidth, (int)imageLength, scanlineSize/2,
				PixelFormat.Format8bppIndexed, bmpData);

			// setup grayscale palette
			ColorPalette palette = bmp.Palette;
			for (int i=0; i<256; i++)
			{
				Color c = Color.FromArgb(i,i,i);
				palette.Entries[i] = c;
			}
			bmp.Palette = palette;

			return bmp;
		}

		private void pbPicture_Click(object sender, System.EventArgs e)
		{
			if (tiff == IntPtr.Zero) return;
			
			DateTime dtStart = DateTime.Now;
			Random rnd = new Random();
			
			// these are the grayscale LUT limits of rescale on click
			int min = rnd.Next(0,20);
			int max = rnd.Next(50,100);
			
			// get actual bitmap data of the Bitmap object
			BitmapData bmpData = ((Bitmap)pbPicture.Image).LockBits(
				new Rectangle(0,0,pbPicture.Image.Width,pbPicture.Image.Height),
				ImageLockMode.ReadWrite,
				PixelFormat.Format8bppIndexed);
			tbLastScan0Ptr.Text = bmpData.Scan0.ToString();
			
			// and free the previously allocated memory
			LibTIFF._free(bmpData.Scan0);
			((Bitmap)pbPicture.Image).UnlockBits(bmpData);

			// dispose object and collect garbage (paranoia?... might be useful when dealing with huge files!)
			pbPicture.Image.Dispose();
			GC.Collect();

			// set new image: same file, differently contrasted
			pbPicture.Image = GetBitmapFromTIFFHandle(tiff,(ushort)min,(ushort)max);
			tbContrastingTime.Text = ((ulong)((TimeSpan)(DateTime.Now - dtStart)).TotalMilliseconds).ToString();
		}

		private void tbCreateTIFF_Click(object sender, System.EventArgs e)
		{
			if (sfdCreateTIFF.ShowDialog() != DialogResult.OK) return;

			DateTime startTime = DateTime.Now;

			// image size
			int width = 1024*Convert.ToInt16(tbSizeFactorX.Text);
			int height = 1024*Convert.ToInt16(tbSizeFactorY.Text);
			short rowsPerStrip = (short)height;

			// create file
			IntPtr newTiff = LibTIFF.Open(sfdCreateTIFF.FileName,"w");
			IntPtr pBuffer;

			if (chkSaveStrips.Checked) rowsPerStrip = Convert.ToInt16(tbSaveRowsPerStrip.Text);
			// set required tag. WATCH THE TYPES!!!
			LibTIFF.SetField(newTiff,(int)TIFFTags.TIFFTAG_IMAGEWIDTH,(short)width);
			LibTIFF.SetField(newTiff,(int)TIFFTags.TIFFTAG_IMAGELENGTH,(short)height);
			LibTIFF.SetField(newTiff,(int)TIFFTags.TIFFTAG_BITSPERSAMPLE,(int)16);
			LibTIFF.SetField(newTiff,(int)TIFFTags.TIFFTAG_SAMPLESPERPIXEL,(short)1);
			LibTIFF.SetField(newTiff,(int)TIFFTags.TIFFTAG_ROWSPERSTRIP,(short)rowsPerStrip);
			LibTIFF.SetField(newTiff,(int)TIFFTags.TIFFTAG_PHOTOMETRIC,(short)TIFFTags.PHOTOMETRIC_MINISBLACK);
			LibTIFF.SetField(newTiff,(int)TIFFTags.TIFFTAG_PLANARCONFIG,(short)TIFFTags.PLANARCONFIG_CONTIG);

			if (chkSaveStrips.Checked) {
				// write strips
				pBuffer = LibTIFF._malloc(width*2*rowsPerStrip);

				// assume raw data to be provided as short[]
				short[] shortbits = new short[width*rowsPerStrip]; 

				// strips
				int strips = height/rowsPerStrip;
				for (int strip = 0; strip < strips; strip++)  
				{  
					for (int y = 0; y < rowsPerStrip; y++) 
					{
						// pixels in line: set to specific gray value
						for (int x=0; x < width; x++) 
						{ 
							// short = -32,768 to 32,767 = signed 16-bit integer 
							// ushort is not supported by Marshal.Copy 
							shortbits[y*width+x] = (short) (10 * y); 
						} 
					}
					// copy data to unsafe context
					Marshal.Copy(shortbits,0,pBuffer,width*rowsPerStrip);
					// ... and write to file
					int ret = LibTIFF.WriteRawStrip(newTiff,(uint)strip,pBuffer,width*2*rowsPerStrip);
					if (ret == -1) 
						Console.WriteLine("writing strip " + strip + " returned " + ret);
				}

			} 
			else 
			{
				// write scanline by scanline
				pBuffer = LibTIFF._malloc(width*2);	// 16 bpp

				// assume raw data to be provided as short[]
				short[] shortbits = new short[width];

				// lines
				for (int y = 0; y < height; y++)  
				{  
					// pixels in line: set to specific gray value
					for (int x=0; x < width; x++) 
					{ 
						// short = -32,768 to 32,767 = signed 16-bit integer 
						// ushort is not supported by Marshal.Copy 
						shortbits[x] = (short) (100 * y); 
					} 
					// copy data to unsafe context
					Marshal.Copy(shortbits,0,pBuffer,width);
					// ... and write to file
					if (!LibTIFF.WriteScanline(newTiff,pBuffer,(uint)y))
					{
						Console.WriteLine("writing line " + y + " failed.");
					}
				}
			}
			
			LibTIFF._free(pBuffer);
			LibTIFF.Close(newTiff);
			MessageBox.Show("Time: " + (DateTime.Now - startTime));
		}
	}
}
