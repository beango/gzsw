using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace gzsw.controller
{
    /// <summary>
    /// 文件处理类
    /// </summary>
    public class FileController : BaseController
    {
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase fileData, string hallNo)
        {
            if (fileData != null)
            {
                try
                {
                    var fileExt = Path.GetExtension(fileData.FileName);
                    const string uploadPath = @"~/Upload";
                    var fileDir = Request.MapPath(uploadPath);
                    if (!Directory.Exists(fileDir))
                    {
                        Directory.CreateDirectory(fileDir);
                    }
                    var fileNum = DateTime.Now.ToString("yyyyMMddhhmmss");
                    var filePath = uploadPath + "/" + hallNo + "_" + fileNum + fileExt;

                    fileData.SaveAs(Request.MapPath(filePath));

                    return Json(new { success = 0, message = "上传文件成功.",fileName= Url.Content(filePath) }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    
                }
                
            }
            return Json(new { success=1,message="上传文件不能为空." }, JsonRequestBehavior.AllowGet);
        }
    }
}
