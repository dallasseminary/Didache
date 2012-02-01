using System;
using System.Data;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;

namespace Didache {
	/// <summary>
	/// Summary description for ImageTools
	/// </summary>
	public class ImageTools {

		public static void ScaleImage(string inputPath, string outputPath, int maxWidth, int maxHeight, long quality) {

			Image resizedImage = ScaleImage(inputPath, maxWidth, maxHeight);

			// generate JPEG stuff
			ImageCodecInfo codecEncoder = GetEncoder("image/jpeg");
			EncoderParameters encoderParams = new EncoderParameters(1);
			EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
			encoderParams.Param[0] = qualityParam;

			resizedImage.Save(outputPath, codecEncoder, encoderParams);

			resizedImage.Dispose();
		}

		public static void ScaleImage(Image inputImage, string outputPath, int maxWidth, int maxHeight, long quality) {
			Image resizedImage = ScaleImage(inputImage, maxWidth, maxHeight);

			SaveAsJpeg(resizedImage, outputPath, quality);

			resizedImage.Dispose();
		}


		public static void SaveAsJpeg(Image inputImage, string outputPath, long quality) {

			// generate JPEG stuff
			ImageCodecInfo codecEncoder = GetEncoder("image/jpeg");
			EncoderParameters encoderParams = new EncoderParameters(1);
			EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
			encoderParams.Param[0] = qualityParam;

			inputImage.Save(outputPath, codecEncoder, encoderParams);
		}

		public static void SaveAsJpeg(Image inputImage, Stream stream, long quality) {

			// generate JPEG stuff
			ImageCodecInfo codecEncoder = GetEncoder("image/jpeg");
			EncoderParameters encoderParams = new EncoderParameters(1);
			EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
			encoderParams.Param[0] = qualityParam;

			inputImage.Save(stream, codecEncoder, encoderParams);
		}

		public static Image CropImage(Image inputImage, int targetW, int targetH, int targetX, int targetY) {
			System.Drawing.Image croppedImage = new System.Drawing.Bitmap(targetW, targetH, inputImage.PixelFormat);

			System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(croppedImage);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			g.PixelOffsetMode = PixelOffsetMode.HighQuality;
			g.DrawImage(inputImage, new Rectangle(0, 0, targetW, targetH), targetX, targetY, targetW, targetH, GraphicsUnit.Pixel);

			return croppedImage;
		}

		public static Image ScaleImage(string inputPath, int maxWidth, int maxHeight) {
			// get original
			Image original = Image.FromFile(inputPath);

			return ScaleImage(original, maxWidth, maxHeight);
		}

		public static Image ScaleImage(Image inputImage, int maxWidth, int maxHeight) {

			// calculate size based on ratio
			Double f = Math.Max((Double)inputImage.Width / (Double)maxWidth, (Double)inputImage.Height / (Double)maxHeight);
			if (f < 1) f = 1;
			int newWidth = (int)Math.Ceiling((Double)inputImage.Width / f);
			int newHeight = (int)Math.Ceiling((Double)inputImage.Height / f);


			//Image resizedImage = new Bitmap(newWidth, newHeight, inputImage.PixelFormat);
			Image resizedImage = new Bitmap(newWidth, newHeight);


			Graphics graphics = Graphics.FromImage(resizedImage);
			graphics.CompositingQuality = CompositingQuality.HighQuality;
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			Rectangle rect = new Rectangle(0, 0, newWidth, newHeight);

			graphics.DrawImage(inputImage, rect);

			inputImage.Dispose();

			return resizedImage;
		}



		private static ImageCodecInfo GetEncoder(string mimeType) {
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
			foreach (ImageCodecInfo codec in codecs) {
				if (codec.MimeType == mimeType) {
					return codec;
				}
			}

			return null;
		}

		// creates a JPEG from the current image (ensures non-indexed pixel formats work)
		public static Image OpenAsNonIndexed(string imagePath) {
			Image finalImage = null;
			Image startImage = Image.FromFile(imagePath);

			if (startImage.PixelFormat == PixelFormat.Format1bppIndexed ||
				startImage.PixelFormat == PixelFormat.Format4bppIndexed ||
				startImage.PixelFormat == PixelFormat.Format8bppIndexed) {
				Bitmap startBitmap = new Bitmap(startImage);
				Bitmap newBitmap = new Bitmap(startImage.Width, startImage.Height, PixelFormat.Format32bppArgb);

				for (int x = 0; x < startImage.Width; x++) {
					for (int y = 0; y < startImage.Height; y++) {
						newBitmap.SetPixel(x, y, startBitmap.GetPixel(x, y));
					}
				}
				startBitmap.Dispose();


				finalImage = (Image)newBitmap;
			} else {
				finalImage = startImage;
			}


			return finalImage;


			/*
			// convert to JPG
			if (imagePath.ToLower().EndsWith("jpg") || imagePath.ToLower().EndsWith("jpeg") ) {
				return Image.FromFile(imagePath);
			} else {
				Image image = Image.FromFile(imagePath);
				Stream stream = new System.IO.MemoryStream();

				ImageTools.SaveAsJpeg(image, stream, 100);
				
				Image newImage = Image.FromStream(stream);
				image.Dispose();
				stream.Dispose();

				return newImage;
			}
			 * */

		}
	}

}