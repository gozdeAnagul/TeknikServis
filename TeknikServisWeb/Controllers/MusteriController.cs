﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using TeknikServis.BLL.Helpers;
using TeknikServis.BLL.Repository;
using TeknikServis.Models.Entities;
using TeknikServis.Models.Enums;
using TeknikServis.Models.ViewModels;
using static TeknikServis.BLL.Identity.MembershipTools;

namespace TeknikServisWeb.Controllers
{
    public class MusteriController : Controller
    {
        // GET: Musteri
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ArizaBildirimi()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ArizaBildirimi(ArizaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var arizaRepo = new ArizaRepository();
                var ariza = new Ariza()
                {
                    MusteriId = HttpContext.User.Identity.GetUserId(),
                    Aciklama = model.Aciklama,
                    MarkaAdi = model.MarkaAdi,
                    ModelAdi = model.ModelAdi,
                    Adres = model.Adres,
                    ArizaOlusturmaTarihi = DateTime.Now,
                   
                };
                arizaRepo.Insert(ariza);
                if (model.PostedFile.Count > 0)
                {
                    model.PostedFile.ForEach(file =>
                    {
                        if (file != null && file.ContentLength > 0)
                        {

                            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            string extName = Path.GetExtension(file.FileName);
                            fileName = StringHelpers.UrlFormatConverter(fileName);
                            fileName += StringHelpers.GetCode();
                            var klasoryolu = Server.MapPath("~/Upload/");
                            var dosyayolu = Server.MapPath("~/Upload/") + fileName + extName;

                            if (!Directory.Exists(klasoryolu))
                                Directory.CreateDirectory(klasoryolu);
                            file.SaveAs(dosyayolu);

                            WebImage img = new WebImage(dosyayolu);
                            img.Resize(250, 250, false);
                            img.Save(dosyayolu);

                            new FotografRepository().Insert(new Fotograf()
                            {
                                ArizaId = ariza.Id,
                                Yol = "/Upload/" + fileName + extName
                            });
                        }
                    });
                }
                var fotograflar= new FotografRepository().GetAll(x => x.ArizaId == ariza.Id).ToList();
                ariza.ArizaFoto = fotograflar.Select(x => x.Yol).ToList();
                arizaRepo.Update(ariza);
               
                TempData["Message"] = "Kaydınız alınlıştır";
                return RedirectToAction("ArizaBildirimi", "Musteri");
            }
            catch (Exception ex)
            {
                TempData["Model"] = new ErrorViewModel()
                {
                    Text = $"Bir hata oluştu {ex.Message}",
                    ActionName = "ArizaBildirimi",
                    ControllerName = "Musteri",
                    ErrorCode = 500
                };
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public ActionResult KayitliArizalar()
        {
            var id = HttpContext.User.Identity.GetUserId();
            var data = new List<ArizaViewModel>();
            var ariza = new ArizaRepository().GetAll(x => x.MusteriId == id).ToList();
            foreach (var x in ariza)
            {
                data.Add(new ArizaViewModel()
                {
                    Id = x.Id,
                    ArizaOlusturmaTarihi = x.ArizaOlusturmaTarihi,
                    TeknisyenAdi = x.Teknisyen?.Name + " " + x.Teknisyen?.Surname,
                    TeknisyenDurumu = x.Teknisyen?.TeknisyenDurumu == null ? TeknisyenDurumu.Beklemede : x.Teknisyen.TeknisyenDurumu,
                    ArizaOnaylandiMi = x.ArizaOnaylandiMi,
                    Ucret = x.Ucret
                });
            }
            return View(data);
        }



        [HttpGet]
        public ActionResult Anket()
        {
            return View();

        }


        public ActionResult Anket (int id)
        {
            return View();
        }
    }
}