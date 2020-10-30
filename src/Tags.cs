////////////////////////////////////////////////////////////////////////////////////////////////////
// ExtractTables.cs
// Copyright (c) 2018 Pdfix. All Rights Reserved.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using PDFixSDK.Pdfix;

namespace PDFix.App.Module
{
    class Tags
    { 
        public static void Run()
        {
            Pdfix pdfix = new Pdfix();
            if (pdfix == null)
                throw new Exception("Pdfix initialization fail");
            PdfDoc doc = pdfix.OpenDoc(@"test.pdf", "");
            if (doc == null)
                throw new Exception(pdfix.GetError());

            //test #1 
            var page = doc.AcquirePage(0);
            var content = page.GetContent();
            Console.WriteLine("--- Checking number of tags ---\n");
            int dem = 1;
            for (int i = 0; i < content.GetNumObjects(); i++)
            {
                var obj = content.GetObject(i);
                var type = obj.GetObjectType();
                if (type == PdfPageObjectType.kPdsPageText)
                {
                    var text = ((PdsText)obj).GetText();
                    var markedContent = obj.GetContentMark();

                    //get text with mcid and number of tags
                    Console.WriteLine(dem + ". Text obj = " + text + " (mcid=" + markedContent.GetTagMcid() + ", num of tags= " + (markedContent.GetNumTags()) + ")");
                    dem++;
                }

            }
            page.Release();
            //end test
            Console.WriteLine("\n--- CLEAN TAGS ---\n");
            doc.RemoveTags(null, IntPtr.Zero);//remove tags

            //test #2 
            page = doc.AcquirePage(0);
            content = page.GetContent();
            Console.WriteLine("--- Checking number of tags again ---\n");
            dem = 1;
            for (int i = 0; i < content.GetNumObjects(); i++)
            {
                var obj = content.GetObject(i);
                var type = obj.GetObjectType();
                if (type == PdfPageObjectType.kPdsPageText)
                {
                    var text = ((PdsText)obj).GetText();
                    var markedContent = obj.GetContentMark();

                    //get text with mcid and number of tags
                    Console.WriteLine(dem + ". Text obj = " + text + " (mcid=" + markedContent.GetTagMcid() + ", num of tags= " + (markedContent.GetNumTags()) + ")");
                    dem++;
                }
            }
            //end test
            doc.Save("tagged.pdf", Pdfix.kSaveFull);
            doc.Close();
            pdfix.Destroy();
        }
    }
}