using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

using MyBookList.Models;

//autentificação por email
using System.Net;
using System.Net.Mail;

//encriptacao
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Web.Security;

namespace MyBookList.Controllers
{
    public class AccountController : Controller
    {
        DataClassesMBLDataContext db = new DataClassesMBLDataContext();

        //View para indicar que foi enviado email
        public ActionResult AtivationEmail(int id)
        {
            Utilizador u = db.Utilizadors.Single(x => x.ID_Utilizador == id);
            return View(u);
        }

        //View para indicar que a conta já está ativa
        public ActionResult AtivationComplete(int id, string token)
        {
            Utilizador u = db.Utilizadors.Single(x => x.ID_Utilizador == id && x.Token == token);
            u.Estado = 1;
            u.Token = null;
            db.SubmitChanges();

            return View(u);
        }

        public ActionResult Registar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registar(FormCollection collection)
        {
            Utilizador u = new Utilizador();

            if (string.IsNullOrEmpty(collection["Nome"]))
                ModelState.AddModelError("Nome", "Tem de preencher o campo!");

            if (string.IsNullOrEmpty(collection["Username"]))
                ModelState.AddModelError("Username", "Tem de preencher o campo!");
            else
            {
                try
                {
                    u = db.Utilizadors.Single(x => x.Username == collection["Username"]);
                }
                catch { }

                if (collection["Username"] == u.Username)
                    ModelState.AddModelError("Username", "Esse Username já não está disponível!");
            }

            if (string.IsNullOrEmpty(collection["Password"]))
                ModelState.AddModelError("Password", "Tem de preencher o campo!");

            if (string.IsNullOrEmpty(collection["Password2"]))
                ModelState.AddModelError("Password2", "Tem de preencher o campo!");
            else
            {
                if (!string.Equals(collection["Password"], collection["Password2"]))
                    ModelState.AddModelError("Password2", "As passwords têm que coincidir!");
            }

            if (string.IsNullOrEmpty(collection["DataNascimento"]))
                ModelState.AddModelError("DataNascimento", "Tem de preencher o campo!");
            else
            {
                DateTime d = Convert.ToDateTime(collection["DataNascimento"]);
                if (d >= DateTime.Now)
                    ModelState.AddModelError("DataNascimento", "Data inválida");
            }

            if (string.IsNullOrEmpty(collection["ContactoEmail"]))
                ModelState.AddModelError("ContactoEmail", "Tem de preencher o campo!");
            else
            {
                try
                {
                    u = db.Utilizadors.Single(x => x.ContactoEmail == collection["ContactoEmail"]);
                }
                catch { }

                if (collection["ContactoEmail"] == u.ContactoEmail)
                    ModelState.AddModelError("ContactoEmail", "Esse email já existe!");
            }


            if (ModelState.IsValid)
            {
                u.Nome = collection["Nome"];
                u.Username = collection["Username"];
                u.Pass = collection["Password"];
                u.Data_Nascimento = Convert.ToDateTime(collection["DataNascimento"]);
                u.ContactoEmail = collection["ContactoEmail"];
                u.Estado = 0; // 0-pendente deve passar para 1-ativo quando tiver confirmação por email
                u.Token = GenerateToken();

                u.Pass = Encrypt(u.Pass);

                db.Utilizadors.InsertOnSubmit(u);
                db.SubmitChanges();

                Auntenticar(u.ContactoEmail, u.Username, u.Token);

                //alterar aqui para ir uma chave que nao de para imitar...
                return RedirectToAction("AtivationEmail", "Account", new { id = u.ID_Utilizador });
            }
            else
            {
                return View(u);
            }

        }

        public ActionResult Login()
        {
            if (Session["admin"] != null)
                return RedirectToAction("Home","BackOffice","");
            
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            if (string.IsNullOrEmpty(collection["Username"]))
                ModelState.AddModelError("Username", "Tem de preencher o campo!");

            if (string.IsNullOrEmpty(collection["Pass"]))
                ModelState.AddModelError("Pass", "Tem de preencher o campo!");

            Utilizador u = new Utilizador();
            Administrador a = new Administrador();
            try
            {
                u = db.Utilizadors.Single(x => x.Username == collection["Username"] && x.Pass == Encrypt(collection["Pass"]));
            }
            catch
            {
                try
                {
                    a = db.Administradors.Single(x => x.Username == collection["Username"] && x.Pass == Encrypt(collection["Pass"]));
                }
                catch
                {
                    ModelState.AddModelError("Username", "Credenciais inválidas.");
                }   
            }

            if(a.Username == null)
            {
                //Estado != 1 -> significa que ou está pendente 0 ou bloqueado 2
                if (u.Estado != 1)
                {
                    ModelState.AddModelError("Username", "Esta conta não está ativa! Contacte administrador");
                }

                if (ModelState.IsValid)
                {
                    FormsAuthentication.SetAuthCookie(u.Username, false);
                    HttpCookie foto = new HttpCookie("Foto");

                    string caminhoFoto1 = HttpContext.Server.MapPath("/Content/Imagens/Users/") + u.ID_Utilizador + ".jpg";
                    string caminhoFoto2 = HttpContext.Server.MapPath("/Content/Imagens/Users/") + u.ID_Utilizador + ".jpeg";
                    string caminhoFoto3 = HttpContext.Server.MapPath("/Content/Imagens/Users/") + u.ID_Utilizador + ".png";

                    if (System.IO.File.Exists(caminhoFoto1))
                    {
                        foto.Value = "/Content/Imagens/Users/" + u.ID_Utilizador + ".jpg";
                    }
                    else
                    {
                        if (System.IO.File.Exists(caminhoFoto2))
                        {
                            foto.Value = "/Content/Imagens/Users/" + u.ID_Utilizador + ".jpeg";
                        }
                        else
                        {
                            if (System.IO.File.Exists(caminhoFoto3))
                            {
                                foto.Value = "/Content/Imagens/Users/" + u.ID_Utilizador + ".png";
                            }
                            else
                            {
                                foto.Value = "/Content/Imagens/Users/defaultUser.png";
                            }
                        }
                    }

                    Response.Cookies.Add(foto);

                    return RedirectToAction("Index", "Registado");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                Session["admin"] = a.Username;

                return RedirectToAction("Home", "BackOffice");
            }

        }

        public ActionResult AlterarPass()
        {
            //encontrar o user
            Utilizador a = new Utilizador();
            a = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);

            return View(a);
        }

        [HttpPost]
        public ActionResult AlterarPass(FormCollection dados)
        {
            Utilizador a = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);

            if (string.IsNullOrEmpty(dados["oldPass"]))
                ModelState.AddModelError("oldPass", "Tem de preencher o campo!");

            if (string.IsNullOrEmpty(dados["newPass"]))
                ModelState.AddModelError("newPass", "Tem de preencher o campo!");

            if (string.IsNullOrEmpty(dados["repeat"]))
                ModelState.AddModelError("repeat", "Tem de preencher o campo!");

            if (!string.Equals(dados["newPass"], dados["repeat"]))
                ModelState.AddModelError("repeat", "As passwords não coincidem!");

            if (!string.Equals(Encrypt(dados["oldPass"]), a.Pass))
                ModelState.AddModelError("oldPass", "Password incorreta!");

            if (ModelState.IsValid)
            {
                a.Pass = Encrypt(dados["newPass"]);
                db.SubmitChanges();

                return RedirectToAction("Perfil", "Registado", "");
            }


            return View(a);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session["admin"] = null;
            return RedirectToAction("Index", "Home");
        }

        public void Auntenticar(string email, string username, string Token)
        {
            Utilizador u = db.Utilizadors.Single(x => x.ContactoEmail == email);

            MailMessage message = new MailMessage();
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;

            NetworkCredential basicCredential = new NetworkCredential("mybooklistmanagement@gmail.com", "Qwertyuiop");
            MailAddress fromAddress = new MailAddress("mybooklistmanagement@gmail.com");

            message.From = fromAddress;
            message.To.Add(email);
            message.Subject = "Confirmação do email de registo";
            message.Body =
                "Estimado " + username + ", <br/>" +
                "<p>O seu pedido de registo no MyBookList necessita de validação. A conta é criada, após este passo.</p>" +
                "<p>Por favor, clique no link abaixo para confirmar o seu endereço de email.</p>" +
                "<li><a href='" + HttpContext.Request.Url.AbsoluteUri.Replace("/Registar", "/Activation/" + u.ID_Utilizador + "/" + u.Token) + "'>Clique aqui para ativar a conta!</a></li>";
            message.IsBodyHtml = true;
            smtpClient.EnableSsl = true;

            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = basicCredential;

            smtpClient.Send(message);


            return;
        }

        public ActionResult ForgotPass()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPass(FormCollection dados)
        {
            if (string.IsNullOrEmpty(dados["Username"]))
                ModelState.AddModelError("Username","Tem de preencher este campo!");

            if (string.IsNullOrEmpty(dados["Email"]))
                ModelState.AddModelError("Email", "Tem de preencher este campo!");

            Utilizador u = new Utilizador();

            try
            {
                u = db.Utilizadors.Single(x => x.Username == dados["Username"] && x.ContactoEmail == dados["Email"]);
            }
            catch
            {
                ModelState.AddModelError("Username", "Utilizador não encontrado!");
            }

            if (ModelState.IsValid)
            {
                if(u.Estado == 1)
                {
                    u.Token = GenerateToken();
                    db.SubmitChanges();

                    RecuperarEmail(u.ContactoEmail, u.Username, u.Token);

                    return RedirectToAction("PasswordEmail");
                }
                
            }

            return View();
        }

        public ActionResult PasswordEmail()
        {
            return View();
        }

        public ActionResult Password(int id, string token)
        {
            Utilizador u = db.Utilizadors.Single(x => x.ID_Utilizador == id && x.Token == token);

            return View(u);
        }

        [HttpPost]
        public ActionResult Password(FormCollection dados)
        {
            Utilizador u = db.Utilizadors.Single(x => x.ID_Utilizador == Convert.ToInt32(dados["id"]));

            if (!string.Equals(dados["newPass"], dados["repeat"]))
                ModelState.AddModelError("newPass", "As passwords têm que coincidir!");

            if (ModelState.IsValid)
            {
                u.Pass = Encrypt(dados["newPass"]);
                u.Token = null;
                db.SubmitChanges();

                return RedirectToAction("Login");
            }

            return View(u);
        }


        public void RecuperarEmail(string email, string username, string Token)
        {
            Utilizador u = db.Utilizadors.Single(x => x.ContactoEmail == email);

            MailMessage message = new MailMessage();
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;

            NetworkCredential basicCredential = new NetworkCredential("mybooklistmanagement@gmail.com", "Qwertyuiop");
            MailAddress fromAddress = new MailAddress("mybooklistmanagement@gmail.com");

            message.From = fromAddress;
            message.To.Add(email);
            message.Subject = "Recuperação de Password";
            message.Body =
                "Estimado " + username + ", <br/>" +
                "<p>Este email foi gerado tendo em vista a recuperação de password.</p>" +
                "<p>Por favor, clique no link abaixo para recuperar o acesso à sua conta.</p>" +
                "<li><a href='" + HttpContext.Request.Url.AbsoluteUri.Replace("/ForgotPass", "/Password/" + u.ID_Utilizador + "/" + u.Token) + "'>Clique aqui para ativar a conta!</a></li>";
            message.IsBodyHtml = true;
            smtpClient.EnableSsl = true;

            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = basicCredential;

            smtpClient.Send(message);


            return;
        }

        public string Encrypt(string str)
        {
            string EncrptKey = "2016;[rmtCQA)MyBookList";
            byte[] byKey = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byKey = System.Text.Encoding.UTF8.GetBytes(EncrptKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(str);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string str)
        {
            str = str.Replace(" ", "+");
            string DecryptKey = "2016;[rmtCQA)MyBookList";
            byte[] byKey = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byte[] inputByteArray = new byte[str.Length];

            byKey = System.Text.Encoding.UTF8.GetBytes(DecryptKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            inputByteArray = Convert.FromBase64String(str.Replace(" ", "+"));
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            return encoding.GetString(ms.ToArray());
        }


        public static string GenerateToken()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
        }


    }
}