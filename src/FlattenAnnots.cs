////////////////////////////////////////////////////////////////////////////////////////////////////
// FlattenAnnots.cs
// Copyright (c) 2018 PDFix. All Rights Reserved.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using PDFixSDK.Pdfix;

namespace PDFix.App.Module
{
    class FlattenAnnots
    {
        public static void Run(
            String openPath,                            // source PDF document
            String savePath,                            // output PDF document
            PdfFlattenAnnotsParams flattenAnnotsParams  // flatten annots params
            )
        {
            Pdfix pdfix = new Pdfix();
            if (pdfix == null)
                throw new Exception("Pdfix initialization fail");

            PdfDoc doc = pdfix.OpenDoc(openPath, "");
            if (doc == null)
                throw new Exception(pdfix.GetError());

            if (!doc.FlattenAnnots(flattenAnnotsParams))
                throw new Exception(pdfix.GetError());

            if (!doc.Save(savePath, Pdfix.kSaveFull))
                throw new Exception(pdfix.GetError());

            doc.Close();
            pdfix.Destroy();
        }
    }
}