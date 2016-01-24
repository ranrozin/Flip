using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageProcessor;
using ImageProcessor.Imaging;
using System.IO;
using System.Windows.Forms;
using ImageProcessor.Imaging.Filters.Photo;
using System.Drawing;

namespace Flip
{
    public class ImageMan
    {
        ImageFactory imFactory;
        ImageProcessor.Imaging.Formats.ISupportedImageFormat format;
        Boolean original;
        Image orgImage;
        public ImageMan()
        {
            try
            {
                imFactory = new ImageFactory(preserveExifData: true);
                original = true;
            }
            catch (Exception ex)
            {
            }
        }
        public Image OrgIamge { get { return orgImage; } }

        public void Load (byte[] imBytes)
        {
            imFactory.Load(imBytes);
            if (original)
            {
                orgImage = new Bitmap(imFactory.Image);
                original = false;
            }
            format = imFactory.CurrentImageFormat;

        }
        public Image getImage()
        {
            return imFactory.Image;
        }

        public void Convert_to_Gs()
        {
            this.Reset();
            IMatrixFilter Ifilter;
            Ifilter = MatrixFilters.GreyScale;
            imFactory.Filter(Ifilter);
        }

        public void Convert_to_BW()
        {
            this.Reset();
            IMatrixFilter Ifilter;
            Ifilter = MatrixFilters.BlackWhite;
            imFactory.Filter(Ifilter);
        }

        public void Convert_to_Comic()
        {
            this.Reset();
            IMatrixFilter Ifilter;
            Ifilter = MatrixFilters.Comic;
            imFactory.Filter(Ifilter);
        }

        public void Convert_to_Sepia()
        {
            this.Reset();
            IMatrixFilter Ifilter;
            Ifilter = MatrixFilters.Sepia;
            imFactory.Filter(Ifilter);
        }
        
        public void Convert_to_LoSatch()
        {
            this.Reset();
            IMatrixFilter Ifilter;
            Ifilter = MatrixFilters.LoSatch;
            imFactory.Filter(Ifilter);
        }

        public void Convert_to_Lomograph()
        {
            this.Reset();
            IMatrixFilter Ifilter;
            Ifilter = MatrixFilters.Lomograph;
            imFactory.Filter(Ifilter);
        }

        public void Convert_to_Gotham()
        {
            this.Reset();
            IMatrixFilter Ifilter;
            Ifilter = MatrixFilters.Gotham;
            imFactory.Filter(Ifilter);
        }

        public void Convert_to_Polaroid()
        {
            this.Reset();
            IMatrixFilter Ifilter;
            Ifilter = MatrixFilters.Polaroid;
            imFactory.Filter(Ifilter);
        }

        public void Convert_to_Invert()
        {
            this.Reset();
            IMatrixFilter Ifilter;
            Ifilter = MatrixFilters.Invert;
            imFactory.Filter(Ifilter);
        }

        public void FlipMe(bool horizontal, bool both = false)
        {
            this.Reset();
            imFactory.Flip(horizontal, both);

        }
        public void Reset()
        {
            imFactory.Load(Utils.imageToByteArray(this.orgImage));
     
        }

        static public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                return ms.ToArray();
            }
        }

    }
}
