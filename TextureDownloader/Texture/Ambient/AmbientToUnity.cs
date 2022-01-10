using Aspose.Zip;
using Aspose.Zip.Saving;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO.Compression;
using System.Text;
using TextureDownloader.Texture.Utils;

namespace TextureDownloader.Texture.Ambient
{
    public class AmbientToUnity
    {
        public delegate void AmbientUnityEventHandler(object sender, AmbientUnityEventArgs e);
        public event AmbientUnityEventHandler AmbientUnityEvent;

        private string file;
        private TextureRessources textureRessources;
        private int state;

        public AmbientToUnity(string file, TextureRessources textureRessources)
        {
            this.file = file;
            this.textureRessources = textureRessources;
        }

        protected virtual void RaiseAmbientUnityEvent(string text, int progress)
        {
            state++;
            AmbientUnityEvent?.Invoke(this, new AmbientUnityEventArgs(text, state, progress));
        }

        public (bool isConverted, string? message) ConvertToUnity()
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            string dir = Path.GetDirectoryName(Path.GetFullPath(file));
            string mainExt = Path.GetExtension(file);
            if (!mainExt.Equals(".zip", StringComparison.CurrentCultureIgnoreCase))
                return (false, "It's not a zip file !");
            RaiseAmbientUnityEvent("ZIP FILE DETECTED", 0);

            var resultValid = Find.isValidExtension(fileName);
            if (!resultValid.isValid)
                return (false, "The files in the zip are not valid extension file !");

            RaiseAmbientUnityEvent($"{resultValid.extFound} TEXTURE FILES DETECTED", 0);

            int uidx = fileName.LastIndexOf('_');
            string shortName = fileName[..(uidx > 0 ? uidx : fileName.Length)];

            try
            {
                RaiseAmbientUnityEvent($"BEGIN EXTRACTION", 0);
                extractAmbientCG(new ZipArchive(File.OpenRead(file), ZipArchiveMode.Read), dir, shortName, resultValid.extFound.ToString().ToLower());
                RaiseAmbientUnityEvent("EXTRACTION FINISHED", 0);
            }
            catch (Exception ex)
            {
                return (false, $"Error processing {fileName}\n{ex}");
            }


            return (true, null);
        }

        private void extractAmbientCG(ZipArchive arc, string dir, string name, string ext)
        {
            static bool tryFindEntry(ZipArchive arc, string suffix, out ZipArchiveEntry e)
            {
                e = arc.Entries.FirstOrDefault(x => x.Name.ToLowerInvariant().EndsWith(suffix)); return e != null;
            }

            static ZipArchiveEntry findEntryOrNull(ZipArchive arc, string suffix)
            {
                tryFindEntry(arc, suffix, out ZipArchiveEntry e); return e;
            }

            static void copyEntry(ZipArchive arc, string suffix, string outFile, bool throwIfNotFound)
            {
                if (File.Exists(outFile)) File.Delete(outFile);
                if (tryFindEntry(arc, suffix, out ZipArchiveEntry e))
                {
                    using Stream IN = e.Open();
                    using Stream OUT = File.OpenWrite(outFile);
                    IN.CopyTo(OUT);
                }
                else if (throwIfNotFound)
                    throw new Exception($"Could not find an entry ending with {suffix} in [{string.Join(", ", arc.Entries.Select(x => x.Name))}]");
            }

            static byte[] readStreamBytes(ZipArchiveEntry e)
            {
                if (e == null) return null;
                using Stream es = e.Open();
                using MemoryStream ms = new();
                es.CopyTo(ms);
                return ms.ToArray();
            }

            string colorOut = $"{dir}/{name}_{textureRessources.Attribute}_alb.{ext}";
            string mosOut = $"{dir}/{name}_{textureRessources.Attribute}_mos.{ext}";
            string normalOut = $"{dir}/{name}_{textureRessources.Attribute}_nml.{ext}";
            string plxOut = $"{dir}/{name}_{textureRessources.Attribute}_plx.{ext}";
            ZipArchiveEntry metalness = findEntryOrNull(arc, $"_metalness.{ext}");
            ZipArchiveEntry roughness = findEntryOrNull(arc, $"_roughness.{ext}");
            ZipArchiveEntry ao = findEntryOrNull(arc, $"_ambientocclusion.{ext}");
            if (File.Exists(mosOut)) File.Delete(mosOut);
            RaiseAmbientUnityEvent($"CREATING MOS MAP", 0);
            makeMosMap(readStreamBytes(metalness), readStreamBytes(roughness), readStreamBytes(ao), mosOut.Replace(ext, "png"));
            RaiseAmbientUnityEvent($"MOS MAP FINISHED", 0);
            RaiseAmbientUnityEvent($"CREATING COLOR", 0);
            copyEntry(arc, $"_color.{ext}", colorOut.Replace(ext, "png"), true);
            RaiseAmbientUnityEvent($"COLOR FINISHED", 0);
            RaiseAmbientUnityEvent($"CREATING NORMAL GL", 0);
            copyEntry(arc, $"_normalgl.{ext}", normalOut.Replace(ext, "png"), true);
            RaiseAmbientUnityEvent($"NORMAL GL FINISHED", 0);
            RaiseAmbientUnityEvent($"CREATING DISPLACEMENT", 0);
            copyEntry(arc, $"_displacement.{ext}", plxOut.Replace(ext, "png"), false);
            RaiseAmbientUnityEvent($"DISPLACEMENT FINISHED", 0);

            RaiseAmbientUnityEvent($"CREATING ZIP FILE", 0);
            using (FileStream zipFile = File.Open($"{dir}/{name}_{textureRessources.Attribute}_unity.zip", FileMode.Create))
            {
                // File to be added to archive
                FileStream colorStream = File.Open(colorOut.Replace(ext, "png"), FileMode.Open, FileAccess.Read);
                FileStream mosStream = File.Open(mosOut.Replace(ext, "png"), FileMode.Open, FileAccess.Read);
                FileStream normalStream = File.Open(normalOut.Replace(ext, "png"), FileMode.Open, FileAccess.Read);
                FileStream plxStream = File.Open(plxOut.Replace(ext, "png"), FileMode.Open, FileAccess.Read);
                using (var archive = new Archive())
                {
                    archive.CreateEntry(colorOut.Replace($"{dir}/", "").Replace(ext, "png"), colorStream);
                    archive.CreateEntry(mosOut.Replace($"{dir}/", "").Replace(ext, "png"), mosStream);
                    archive.CreateEntry(normalOut.Replace($"{dir}/", "").Replace(ext, "png"), normalStream);
                    archive.CreateEntry(plxOut.Replace($"{dir}/", "").Replace(ext, "png"), plxStream);
                    archive.Save(zipFile);
                }
                colorStream.Close();
                mosStream.Close();
                normalStream.Close();
                plxStream.Close();
            }
            arc.Dispose();
            File.Delete(file);
            File.Delete(colorOut.Replace(ext, "png"));
            File.Delete(mosOut.Replace(ext, "png"));
            File.Delete(normalOut.Replace(ext, "png"));
            File.Delete(plxOut.Replace(ext, "png"));
        }

        private void makeMosMap(byte[] metalBytes, byte[] roughBytes, byte[] aoBytes, string outFile)
        {
            static Bitmap bytesToBitmap(byte[] bytes)
            {
                if (bytes == null) return null;
                using MemoryStream ms = new(bytes);
                return new Bitmap(ms, false);
            }

            // https://stackoverflow.com/questions/1922040/how-to-resize-an-image-c-sharp
            static Bitmap resize(Bitmap b, int w, int h)
            {
                Rectangle destRect = new(0, 0, w, h);
                Bitmap destImage = new(w, h);
                using var graphics = Graphics.FromImage(destImage);
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                using ImageAttributes wrapMode = new();
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                graphics.DrawImage(b, destRect, 0, 0, b.Width, b.Height, GraphicsUnit.Pixel, wrapMode);
                return destImage;
            }

            static void matchSizes(ref Bitmap a, ref Bitmap b, ref Bitmap c, out int w, out int h)
            {
                w = 0; h = 0;
                if (a != null) { w = Math.Max(w, a.Width); h = Math.Max(h, a.Height); }
                if (b != null) { w = Math.Max(w, b.Width); h = Math.Max(h, b.Height); }
                if (c != null) { w = Math.Max(w, c.Width); h = Math.Max(h, c.Height); }
                if (a != null && (a.Width != w || a.Height != h)) { Bitmap n = resize(a, w, h); a.Dispose(); a = n; }
                if (b != null && (b.Width != w || b.Height != h)) { Bitmap n = resize(b, w, h); b.Dispose(); b = n; }
                if (c != null && (c.Width != w || c.Height != h)) { Bitmap n = resize(c, w, h); c.Dispose(); c = n; }
            }

            // TODO this is MUCH slower than it needs to be; see LockPixels()
            Color[] readColors(Bitmap bmp, int w, int h)
            {
                Color[] a = new Color[w * h];
                int totalColor = a.Length;
                for (int y = 0; y < h; ++y)
                {
                    for (int x = 0; x < w; ++x)
                        a[y * w + x] = bmp.GetPixel(x, y);
                    RaiseAmbientUnityEvent($"READING COLORS", (int)(((float)y * (float)w / (float)totalColor) * 100f));
                }
                RaiseAmbientUnityEvent($"READING COLORS", 100);

                return a;
            }

            // TODO this is MUCH slower than it needs to be; see LockPixels()
            void writeColors(Bitmap bmp, int w, int h, Color[] a)
            {
                int totalColor = a.Length;
                for (int y = 0; y < h; ++y)
                {
                    for (int x = 0; x < w; ++x)
                        bmp.SetPixel(x, y, a[(y * w) + x]);
                    RaiseAmbientUnityEvent($"WRITING COLORS", (int)(((float)y * (float)w / (float)totalColor) * 100f));
                }

                RaiseAmbientUnityEvent($"WRITING COLORS", 100);
            }

            static Color[] fakeColors(int len, int b)
            {
                Color[] a = new Color[len];
                for (int i = 0; i < len; ++i)
                    a[i] = Color.FromArgb(255, b, b, b);
                return a;
            }

            static Color[] combineMosColors(Color[] metal, Color[] rough, Color[] ao)
            {
                System.Diagnostics.Debug.Assert(metal != null && rough != null && ao != null && metal.Length > 0 && rough.Length == metal.Length && ao.Length == metal.Length);
                int len = metal.Length;
                Color[] mos = new Color[len];
                for (int i = 0; i < len; ++i)
                    mos[i] = combineMosColor(metal[i], rough[i], ao[i]);
                return mos;
            }

            // this is the unity mask map format -- red is metallic, green is AO, and alpha is smoothness (just inverted
            // roughness). In HDRP and Better Lit, the Blue channel is used for detail mask. Currently, we are not using
            // the detail mask at all, and could possibly repurpose this for parallax or something, but for now I'm leaving
            // it just as 0 in case some materials need detial masks someday and to keep compatibility with other shaders
            // Note that Better Lit uses albedo alpha for parallax and URP default needs a separate parallax map, so there's no
            // real standard for where parallax should be.
            static Color combineMosColor(Color metal, Color rough, Color ao) =>
                Color.FromArgb(
                    red: metal.R,
                    green: ao.R,
                    blue: 0,
                    alpha: 255 - rough.R);

            Bitmap metalBmp = null, roughBmp = null, aoBmp = null;
            Color[] metalColors, roughColors, aoColors;
            int width, height;
            try
            {
                metalBmp = bytesToBitmap(metalBytes);
                roughBmp = bytesToBitmap(roughBytes);
                aoBmp = bytesToBitmap(aoBytes);
                matchSizes(ref metalBmp, ref roughBmp, ref aoBmp, out width, out height);
                metalColors = metalBmp != null ? readColors(metalBmp, width, height) : fakeColors(width * height, 0);
                roughColors = roughBmp != null ? readColors(roughBmp, width, height) : fakeColors(width * height, 127);
                aoColors = aoBmp != null ? readColors(aoBmp, width, height) : fakeColors(width * height, 255);
            }
            finally
            {
                metalBmp?.Dispose();
                roughBmp?.Dispose();
                aoBmp?.Dispose();
            }

            using Bitmap mosBmp = new(width, height);
            writeColors(mosBmp, width, height, combineMosColors(metalColors, roughColors, aoColors));
            mosBmp.Save(outFile);
        }
    }

    public class AmbientUnityEventArgs
    {
        public AmbientUnityEventArgs(string text, int state, int progress) { Text = text; State = state; Progress = progress; }
        public string Text { get; }
        public int State { get; }
        public int Progress { get; }
    }
}
