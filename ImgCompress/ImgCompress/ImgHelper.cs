using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ImgCompress
{
    public static class ImageHelper
    {
        /// <summary>
        /// 是否为图片
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsImg(string name)
        {

            var ilist = new List<string> {
            ".jpg",".png",".jpeg",".gif",
            };
            return ilist.Where(i => name.ToLower().Contains(i)).Count() > 0 ? true : false;

        }


        ///
        /// 根据指定尺寸得到按比例缩放的尺寸,返回true表示以更改尺寸
        ///
        /// 图片宽度
        /// 图片高度
        /// 指定宽度
        /// /// 指定高度
        /// 返回true表示以更改尺寸
        public static bool GetPicZoomSize(ref int picWidth, ref int picHeight, int specifiedWidth, int specifiedHeight)
        {
            int sW = 0, sH = 0;
            Boolean isZoomSize = false;
            //按比例缩放
            System.Drawing.Size tem_size = new System.Drawing.Size(picWidth, picHeight);
            if (tem_size.Width > specifiedWidth || tem_size.Height > specifiedHeight) //将**改成c#中的或者操作符号
            {
                if ((tem_size.Width * specifiedHeight) > (tem_size.Height * specifiedWidth))
                {
                    sW = specifiedWidth;
                    sH = (specifiedWidth * tem_size.Height) / tem_size.Width;
                }
                else
                {
                    sH = specifiedHeight;
                    sW = (tem_size.Width * specifiedHeight) / tem_size.Height;
                }
                isZoomSize = true;
            }
            else
            {
                sW = tem_size.Width;
                sH = tem_size.Height;
            }
            picHeight = sH;
            picWidth = sW;
            return isZoomSize;
        }
        /// <summary>
        ///  无损压缩图片
        /// </summary>
        /// <param name="sFile"></param>
        /// <param name="dFile"></param>
        /// <param name="HWratio">高宽比例压缩</param>
        /// <param name="flag">质量压缩1 -100</param>
        /// <returns></returns>



        #region Azure blob 等比压缩
             
        private static ImageFormat GetEncoder(string extension)
        {
            ImageFormat encoder = null;

            extension = extension.Replace(".", "");

            var isSupported = Regex.IsMatch(extension, "gif|png|jpe?g", RegexOptions.IgnoreCase);

            if (isSupported)
            {
                switch (extension.ToLower())
                {
                    case "png":
                        encoder = ImageFormat.Png;
                        break;
                    case "jpg":
                        encoder = ImageFormat.Jpeg;
                        break;
                    case "jpeg":
                        encoder = ImageFormat.Jpeg;
                        break;
                    case "gif":
                        encoder = ImageFormat.Gif;
                        break;
                    default:
                        break;
                }
            }

            return encoder;
        }

        public static void resizeImage(string Url, Stream sFile, MemoryStream output, Size size)
        {
            using (System.Drawing.Image imgToResize = System.Drawing.Image.FromStream(sFile))
            {
                //Get the image current width  
                int sourceWidth = imgToResize.Width;
                //Get the image current height  
                int sourceHeight = imgToResize.Height;
                float nPercent = 0;
                float nPercentW = 0;
                float nPercentH = 0;
                //Calulate  width with new desired size  
                nPercentW = ((float)size.Width / (float)sourceWidth);
                //Calculate height with new desired size  
                nPercentH = ((float)size.Height / (float)sourceHeight);
                if (nPercentH < nPercentW)
                    nPercent = nPercentH;
                else
                    nPercent = nPercentW;
                //New Width  
                int destWidth = (int)(sourceWidth * nPercent);
                //New Height  
                int destHeight = (int)(sourceHeight * nPercent);
                var extension = Path.GetExtension(Url);
                using (Bitmap b = new Bitmap(destWidth, destHeight))
                {
                    Graphics g = Graphics.FromImage((System.Drawing.Image)b);
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    // Draw image with new width and height  
                    g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
                    g.Dispose();
                    b.Save(output, GetEncoder(extension));
                }

            }
        }


        #endregion


        public static void Resize(string Url, Stream sFile, MemoryStream output, int width, int height)
        {
            using (System.Drawing.Image image = System.Drawing.Image.FromStream(sFile))
            {
                var destRect = new Rectangle(0, 0, width, height);
                using (var destImage = new Bitmap(width, height))
                {

                    destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                    using (var graphics = Graphics.FromImage(destImage))
                    {
                        graphics.CompositingMode = CompositingMode.SourceCopy;
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        using (var wrapMode = new ImageAttributes())
                        {
                            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                        }
                    }
                }
                var extension = Path.GetExtension(Url);
                image.Save(output, GetEncoder(extension));
            }
        }

    }
}
