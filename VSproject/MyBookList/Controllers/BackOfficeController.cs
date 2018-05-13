using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using PagedList;
using MyBookList.Models;
namespace MyBookList.Controllers
{
    public class BackOfficeController : Controller
    {
        public static List<Disponivel> listlojas = new List<Disponivel>();
        DataClassesMBLDataContext db = new DataClassesMBLDataContext();
        public static string modo;
        // GET: BackOffice
        public ActionResult Home()
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }

            return View(db.Livros);
        }
        [HttpPost]
        public ActionResult editcat(int id)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            ViewBag.id = id;
            ViewBag.valor = db.Categorias.Single(x => x.ID_Categoria == id).Nome;
            return PartialView("tbcat", db.Livros);
        }
        [HttpPost]
        public ActionResult editaut(int id)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            ViewBag.id = id;
            ViewBag.valor = db.Autors.Single(x => x.ID_Autor == id).Nome;
            return PartialView("tbaut", db.Livros);
        }
        [HttpPost]
        public ActionResult guardarcat(int id,string valor)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            db.Categorias.Single(x => x.ID_Categoria ==id).Nome = valor;
            try { 
            db.SubmitChanges();
            }
            catch
            {
                return new HttpStatusCodeResult(410, "Categoria Repetida!");
            }
            return PartialView("Categorias");
        }
        [HttpPost]
        public ActionResult guardaraut(int id, string valor)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            db.Autors.Single(x => x.ID_Autor == id).Nome = valor;
            try
            {
                db.SubmitChanges();
            }
            catch
            {
                return new HttpStatusCodeResult(410, "Autor Repetido!");
            }
            return PartialView("Categorias");
        }

        public ActionResult Eloja(string id)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            ViewBag.id = id;
            ViewBag.valor = db.Lojas.Single(x => x.ID_Loja == Convert.ToInt32(id)).Nome;
            return PartialView("tbloja");
        }
        public string Gloja(FormCollection dados)
        {
            db.Lojas.Single(x => x.ID_Loja == Convert.ToInt32(dados["id"])).Nome = dados["nome"];
            try
            {
                db.SubmitChanges();
            }
            catch
            {
                ModelState.AddModelError("nome", "Loja já existente.");
            }
            return dados["nome"];
            
        }
        public ActionResult Eautor(string idd)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            int id = Convert.ToInt32(idd);
            ViewBag.id = db.Autors.Single(x => x.ID_Autor == id).ID_Autor;
            ViewBag.nome = db.Autors.Single(x =>x.ID_Autor == id).Nome;
            ViewBag.biografia = db.Autors.Single(x => x.ID_Autor == id).Biografia;
            ViewBag.pseudo = db.Autors.Single(x => x.ID_Autor == id).Pseudonimo;
            return PartialView("tbautor");
        }
        public ActionResult Gautor(FormCollection dados)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            if (string.IsNullOrEmpty(dados["nome"]))
            {
                ModelState.AddModelError("nome", "Nome não inserido!");
            }

            

            Autor c = db.Autors.Single(x => x.ID_Autor == Convert.ToInt32(dados["id"]));
            if (ModelState.IsValid)
            {
                c.Nome = dados["nome"];
                c.Pseudonimo = dados["pseudo"];
                c.Biografia = dados["biografia"];
                db.SubmitChanges();
                int id = c.ID_Autor;
                ViewBag.id = c.ID_Autor;
                ViewBag.Nome = c.Nome;
                ViewBag.Biografia = c.Biografia;
                ViewBag.Pseudonimo = c.Pseudonimo;
                return PartialView("Autor", db.Livros.Where(x => x.Estado == true).Where(x => x.Escreve.ID_Autor == c.ID_Autor).OrderBy(x => x.Adiciona.Data_Adicao));
            }
            else return PartialView("Autor", db.Livros.Where(x => x.Estado == true).Where(x => x.Escreve.ID_Autor == c.ID_Autor).OrderBy(x => x.Adiciona.Data_Adicao));
        }
    

        public ActionResult ListLivros()
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            modo = "lista";

            return PartialView(db.Livros);
        }

        public ActionResult ListLivrosPrincipal()
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            modo = "grelha";
            return PartialView(db.Livros);
        }

        public ActionResult ListUtilizadores()
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            return PartialView(db.Utilizadors);
        }
        public ActionResult ListLojas()
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            return PartialView(db.Lojas);
        }
        public ActionResult VerLivro(int id)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            Livro L = db.Livros.Single(x => x.ID_Livro == id);
            ViewBag.CapaLivro = "/Content/Imagens/Livros/" + id + ".jpg";
            ViewBag.Autor = "/Home/Autor/" + L.Escreve.Autor.ID_Autor;
            return PartialView("VerLivro", L);
        }

        public ActionResult EditBook(int id)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            Livro L = db.Livros.Single(x => x.ID_Livro == id);
            ViewBag.Lojas = new SelectList(db.Lojas, "ID_Loja", "Nome");
            ViewBag.Autores = new SelectList(db.Autors, "ID_Autor", "Nome", db.Livros.Single(x => x.ID_Livro == id).Escreve.ID_Livro);
            ViewBag.Categorias = new SelectList(db.Categorias, "ID_Categoria", "Nome", db.Livros.Single(x => x.ID_Livro == id).Pertence.Categoria.ID_Categoria);
            return View(db.Livros.Single(x => x.ID_Livro == id));
        }
        [HttpPost]
        public ActionResult EditBook(int id, FormCollection dados)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            if (string.IsNullOrEmpty(dados["titulo"]))
                ModelState.AddModelError("titulo", "Não inseriu o título.");
            if (string.IsNullOrEmpty(dados["isbn"]))
                ModelState.AddModelError("isbn", "Não inseriu o ISBN.");
            else if (dados["isbnm"].Length != 9 && dados["isbnm"].Length != 13)
                ModelState.AddModelError("isbnm", "Número de caracteres incorreto.");
            else if (db.Livros.Where(x => x.ISBN == dados["isbnm"]).Count() > 0)
                ModelState.AddModelError("isbnm", "ISBN já existente.");
            if (string.IsNullOrEmpty(dados["autor"]))
                ModelState.AddModelError("autor", "Não inseriu Autor.");
            if (string.IsNullOrEmpty(dados["editora"]))
                ModelState.AddModelError("editora", "Não inseriu a Editora do livro.");
            if (string.IsNullOrEmpty(dados["ednumero"]))
                ModelState.AddModelError("ednumero", "Não inseriu o número de edição.");
            if (string.IsNullOrEmpty(dados["eddata"]))
                ModelState.AddModelError("eddata", "Não inseriu a data de edição.");
            if (string.IsNullOrEmpty(dados["categoria"]) || dados["categoria"] == "--Selecione a Categoria--")
                ModelState.AddModelError("categoria", "Não inseriu a Categoria.");

            if (string.IsNullOrEmpty(dados["autor"]) || dados["autor"] == "--Selecione o Autor--")
                ModelState.AddModelError("autor", "Não inseriu o Autor.");

            if (string.IsNullOrEmpty(dados["sinopse"]))
                ModelState.AddModelError("sinopse", "Não inseriu a Sinópse do livro.");
            try
            {
                if (Convert.ToDateTime(dados["eddata"]) > DateTime.Now)
                    ModelState.AddModelError("eddata", "Data não é válida.");
            }
            catch
            {
                ModelState.AddModelError("eddata", "Data não é válida.");
            }
            ViewBag.Lojas = new SelectList(db.Lojas, "ID_Loja", "Nome");
            ViewBag.Autores = new SelectList(db.Autors, "ID_Autor", "Nome");
            ViewBag.Categorias = new SelectList(db.Categorias, "ID_Categoria", "Nome");
            if (ModelState.IsValid)
            {
                Livro l = db.Livros.Single(x => x.ID_Livro == id);
                l.Titulo = dados["titulo"];
                l.ISBN = dados["isbnm"];
                l.Sinopse = dados["sinopse"];
                l.Editora = dados["editora"];
                l.EdicaoNumero = Convert.ToInt32(dados["ednumero"]);
                l.EdicaoData = Convert.ToDateTime(dados["eddata"]);
                l.Estado = true;
                db.SubmitChanges();

                Escreve e = new Escreve();
                e.ID_Autor = Convert.ToInt32(dados["autor"]);
                e.ID_Livro = l.ID_Livro;
                db.Escreves.InsertOnSubmit(e);

                Pertence p = new Pertence();
                p.ID_Livro = l.ID_Livro;
                p.ID_Categoria = Convert.ToInt32(dados["categoria"]);
                db.Pertences.InsertOnSubmit(p);
                foreach (Disponivel d in listlojas)
                {
                    d.ID_Livro = l.ID_Livro;
                }
                db.SubmitChanges();
                string a = Server.MapPath("~/Content/Imagens/Admin/Temp/book.jpg");
                FileInfo file = new FileInfo(a);
                string b = Server.MapPath("~/Content/Imagens/Livros/book.jpg");
                FileInfo dest = new FileInfo(b);
                if (file.Exists)
                {
                    file.MoveTo(dest.Directory.ToString() + "\\" + l.ID_Livro + ".jpg");
                }

                TempData["EditarLivro"] = l.Titulo + " foi adicionado aos livros com sucesso!";
                return PartialView("EditarLivro");
            }
            else
                return PartialView("EditarLivro", db.Livros.Single(x => x.ID_Livro == id));
        }

        public ActionResult EditAutor(int id)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            Autor A = db.Autors.Single(x => x.ID_Autor == id);
            ViewBag.ImagemAutor = "/Content/Imagens/Autores/" + id + ".jpg";
            return View(A);
        }
        public ActionResult AddCategoria()
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            return PartialView();
        }
        [HttpPost]
        public ActionResult AddCategoria(FormCollection dados)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            if (string.IsNullOrEmpty(dados["nome"]))
            {
                return new HttpStatusCodeResult(410, "Nome não inserido!");
            }
            if (db.Categorias.Where(x => x.Nome == dados["nome"]).Count() > 0)
            {
                return new HttpStatusCodeResult(410, "Categoria já existente!");
            }
            else
            {
                Categoria c = new Categoria();
                c.Nome = dados["nome"];
                db.Categorias.InsertOnSubmit(c);
                db.SubmitChanges();
                return PartialView("Categorias");
            }

        }

        public ActionResult AddAutor()
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            return PartialView();
        }
        [HttpPost]
        public ActionResult AddAutor(FormCollection dados)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            if (string.IsNullOrEmpty(dados["nome"]))
            {
                return new HttpStatusCodeResult(410, "Nome não inserido!");
            }
            if (db.Categorias.Where(x => x.Nome == dados["nome"]).Count() > 0)
            {
                return new HttpStatusCodeResult(410, "Autor já existente!");
            }
            if (string.IsNullOrEmpty(dados["pseudonimo"]))
            {
                return new HttpStatusCodeResult(410, "Pseudónimo não inserido!");
            }
            if (string.IsNullOrEmpty(dados["biografia"]))
            {
                return new HttpStatusCodeResult(410, "Biografia não inserida!");
            }
            else
            {
                Autor c = new Autor();
                c.Nome = dados["nome"];
                c.Pseudonimo = dados["pseudonimo"];
                c.Biografia = dados["biografia"];
                db.Autors.InsertOnSubmit(c);
                db.SubmitChanges();
                return PartialView("Categorias"); //contem os autores também
            }

        }

        public ActionResult AddLoja()
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            return PartialView();
        }
        [HttpPost]
        public ActionResult AddLoja(FormCollection dados)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            if (string.IsNullOrEmpty(dados["nome"]))
            {
                return new HttpStatusCodeResult(410, "Nome não inserido!");
            }
            if (db.Categorias.Where(x => x.Nome == dados["nome"]).Count() > 0)
            {
                return new HttpStatusCodeResult(410, "Loja já existente!");
            }
            else
            {
                ViewBag.Lojas = new SelectList(db.Lojas, "ID_Loja", "Nome");
                Loja c = new Loja();
                c.Nome = dados["nome"];
                db.Lojas.InsertOnSubmit(c);
                db.SubmitChanges();
                return PartialView("atualizarloja");
            }
        }

        public ActionResult atualizarcat()
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            ViewBag.Categorias = new SelectList(db.Categorias, "ID_Categoria", "Nome");
            return PartialView();
        }
        public ActionResult atualizarautor()
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            ViewBag.Autores = new SelectList(db.Autors, "ID_Autor", "Nome");
            return PartialView();
        }

        public ActionResult CreateBook()
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            string a = Server.MapPath("~/Content/Imagens/Admin/Temp/book.jpg");
            FileInfo file = new FileInfo(a);
            file.Delete();
            listlojas.Clear();
            ViewBag.Lojas = new SelectList(db.Lojas, "ID_Loja", "Nome");
            ViewBag.Autores = new SelectList(db.Autors, "ID_Autor", "Nome");
            ViewBag.Categorias = new SelectList(db.Categorias, "ID_Categoria", "Nome");
            return View();
        }

        [HttpPost]
        public ActionResult CreateBook(FormCollection dados)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            ViewBag.Lojas = new SelectList(db.Lojas, "ID_Loja", "Nome");
            ViewBag.Autores = new SelectList(db.Autors, "ID_Autor", "Nome");
            ViewBag.Categorias = new SelectList(db.Categorias, "ID_Categoria", "Nome");
            return View();
        }

        [HttpPost]
        public ActionResult CreateBookInfo(FormCollection dados)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            if (string.IsNullOrEmpty(dados["titulo"]))
                ModelState.AddModelError("titulo", "Não inseriu o título.");
            if (string.IsNullOrEmpty(dados["isbn"]))
                ModelState.AddModelError("isbn", "Não inseriu o ISBN.");
            else if (dados["isbnm"].Length != 9 && dados["isbnm"].Length != 13)
                ModelState.AddModelError("isbnm", "Número de caracteres incorreto.");
            else if (db.Livros.Where(x => x.ISBN == dados["isbnm"]).Count() > 0)
                ModelState.AddModelError("isbnm", "ISBN já existente.");
            if (string.IsNullOrEmpty(dados["autor"]))
                ModelState.AddModelError("autor", "Não inseriu Autor.");
            if (string.IsNullOrEmpty(dados["editora"]))
                ModelState.AddModelError("editora", "Não inseriu a Editora do livro.");
            if (string.IsNullOrEmpty(dados["ednumero"]))
                ModelState.AddModelError("ednumero", "Não inseriu o número de edição.");
            if (string.IsNullOrEmpty(dados["eddata"]))
                ModelState.AddModelError("eddata", "Não inseriu a data de edição.");
            if (string.IsNullOrEmpty(dados["categoria"]) || dados["categoria"] == "--Selecione a Categoria--")
                ModelState.AddModelError("categoria", "Não inseriu a Categoria.");

            if (string.IsNullOrEmpty(dados["autor"]) || dados["autor"] == "--Selecione o Autor--")
                ModelState.AddModelError("autor", "Não inseriu o Autor.");

            if (string.IsNullOrEmpty(dados["sinopse"]))
                ModelState.AddModelError("sinopse", "Não inseriu a Sinópse do livro.");
            try
            {
                if (Convert.ToDateTime(dados["eddata"]) > DateTime.Now)
                    ModelState.AddModelError("eddata", "Data não é válida.");
            }
            catch
            {
                ModelState.AddModelError("eddata", "Data não é válida.");
            }

            ViewBag.Lojas = new SelectList(db.Lojas, "ID_Loja", "Nome");
            ViewBag.Autores = new SelectList(db.Autors, "ID_Autor", "Nome");
            ViewBag.Categorias = new SelectList(db.Categorias, "ID_Categoria", "Nome");
            if (ModelState.IsValid)
            {
                Livro l = new Livro();
                l.Titulo = dados["titulo"];
                l.ISBN = dados["isbnm"];
                l.Sinopse = dados["sinopse"];
                l.Editora = dados["editora"];
                l.EdicaoNumero = Convert.ToInt32(dados["ednumero"]);
                l.EdicaoData = Convert.ToDateTime(dados["eddata"]);
                l.Estado = true;
                db.Livros.InsertOnSubmit(l);
                db.SubmitChanges();

                Escreve e = new Escreve();
                e.ID_Autor = Convert.ToInt32(dados["autor"]);
                e.ID_Livro = l.ID_Livro;
                db.Escreves.InsertOnSubmit(e);

                Pertence p = new Pertence();
                p.ID_Livro = l.ID_Livro;
                p.ID_Categoria = Convert.ToInt32(dados["categoria"]);
                db.Pertences.InsertOnSubmit(p);
                //FALTA O ADICIONA
                foreach (Disponivel d in listlojas)
                {
                    d.ID_Livro = l.ID_Livro;
                    db.Disponivels.InsertOnSubmit(d);
                }
                db.SubmitChanges();

                string a = Server.MapPath("~/Content/Imagens/Admin/Temp/book.jpg");
                FileInfo file = new FileInfo(a);
                string b = Server.MapPath("~/Content/Imagens/Livros/book.jpg");
                FileInfo dest = new FileInfo(b);
                if (file.Exists)
                {
                    file.MoveTo(dest.Directory.ToString() + "\\" + l.ID_Livro + ".jpg");
                }

                TempData["Addlivro"] = l.Titulo + " foi adicionado aos livros com sucesso!";
                return PartialView("AddLivro");
            }
            else
                return PartialView("AddLivro", dados);

        }


        public ActionResult CreateAuthor()
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            string a = Server.MapPath("~/Content/Imagens/Admin/Temp/autor.jpg");
            FileInfo file = new FileInfo(a);
            file.Delete();
            return View();
        }

        [HttpPost]
        public ActionResult CreateAuthor(FormCollection dados)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateAuthorInfo(FormCollection dados)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            if (string.IsNullOrEmpty(dados["nome"]))
            {
                ModelState.AddModelError("nome", "Nome não inserido!");
            }
            if (db.Categorias.Where(x => x.Nome == dados["nome"]).Count() > 0)
            {
                ModelState.AddModelError("nome", "Autor já existente!");
            }
            if (string.IsNullOrEmpty(dados["pseudonimo"]))
            {
                ModelState.AddModelError("pseudonimo", "Pseudónimo não inserido!");
            }
            if (string.IsNullOrEmpty(dados["biografia"]))
            {
                ModelState.AddModelError("biografia", "Biografia não inserida!");
            }

            if (ModelState.IsValid)
            {
                Autor c = new Autor();
                c.Nome = dados["nome"];
                c.Pseudonimo = dados["pseudonimo"];
                c.Biografia = dados["biografia"];
                db.Autors.InsertOnSubmit(c);
                db.SubmitChanges();

                string a = Server.MapPath("~/Content/Imagens/Admin/Temp/autor.jpg");
                FileInfo file = new FileInfo(a);
                string b = Server.MapPath("~/Content/Imagens/Autores/autor.jpg");
                FileInfo dest = new FileInfo(b);
                if (file.Exists)
                {
                    file.MoveTo(dest.Directory.ToString() + "\\" + c.ID_Autor + ".jpg");
                }

                TempData["Addautor"] = c.Nome + " foi adicionado aos autores com sucesso!";
                return PartialView("Cautor");
            }
            return PartialView("Cautor", dados);
        }

        public ActionResult CreateStore()
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            string a = Server.MapPath("~/Content/Imagens/Admin/Temp/loja.jpg");
            FileInfo file = new FileInfo(a);
            file.Delete();
            return View();
        }

        [HttpPost]
        public ActionResult CreateStore(FormCollection dados)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateStoreInfo(FormCollection dados)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            if (string.IsNullOrEmpty(dados["nome"]))
            {
                ModelState.AddModelError("nome", "Nome não inserido!");
            }
            if (db.Categorias.Where(x => x.Nome == dados["nome"]).Count() > 0)
            {
                ModelState.AddModelError("nome", "Loja já existente!");
            }

            if (ModelState.IsValid)
            {
                ViewBag.Lojas = new SelectList(db.Lojas, "ID_Loja", "Nome");
                Loja c = new Loja();
                c.Nome = dados["nome"];
                db.Lojas.InsertOnSubmit(c);
                db.SubmitChanges();

                string a = Server.MapPath("~/Content/Imagens/Admin/Temp/loja.jpg");
                FileInfo file = new FileInfo(a);
                string b = Server.MapPath("~/Content/Imagens/Lojas/loja.jpg");
                FileInfo dest = new FileInfo(b);
                if (file.Exists)
                {
                    file.MoveTo(dest.Directory.ToString() + "\\" + c.ID_Loja + ".jpg");
                }


                TempData["Addloja"] = c.Nome + " foi adicionada ás lojas com sucesso!";
                return PartialView("Cloja");
            }
            return PartialView("Cloja", dados);
        }


        public ActionResult isbn(string isbn)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            if (isbn == "ISBN 9")
            {
                return PartialView("isbn9");
            }
            if (isbn == "ISBN 13")
            {
                return PartialView("isbn13");
            }
            return null;

        }

        public ActionResult AddLink(string link, string loja)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            if (string.IsNullOrEmpty(loja) || loja == "--Selecione a Loja--")
                return new HttpStatusCodeResult(410, "Loja não inserida!");
            if (string.IsNullOrEmpty(link))
                return new HttpStatusCodeResult(410, "Link não inserido!");

            Disponivel d = new Disponivel();
            d.ID_Loja = db.Lojas.Single(x => x.Nome == loja).ID_Loja;

            if (listlojas.Where(x => x.ID_Loja == d.ID_Loja).Count() > 0)
                return new HttpStatusCodeResult(410, "Já inseriu o link para esta loja!");
            d.Link = link;
            ViewBag.link = d;
            ViewBag.loja = loja;
            listlojas.Add(d);
            return PartialView();

        }

        public ActionResult Index(int? pagina)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            int TamanhoPagina;
            string Ordem;

            if (HttpContext.Request.Cookies["livrosPorPagina"] != null)
                TamanhoPagina = Convert.ToInt32(HttpContext.Request.Cookies["livrosPorPagina"].Value);
            else
                TamanhoPagina = 20;

            int NumeroPagina = pagina ?? 1;

            if (HttpContext.Request.Cookies["OrdenarPagina"] != null)
            {
                Ordem = HttpContext.Request.Cookies["OrdenarPagina"].Value;
                if (Ordem == "Data de Edição")
                {
                    ViewBag.title = "Ordenado por Data de Edição:";
                    return View(db.Livros.Where(x => x.Estado == true).OrderBy(x => x.EdicaoData).ToPagedList(NumeroPagina, TamanhoPagina));
                }
                if (Ordem == "ISBN")
                {
                    ViewBag.title = "Ordenado por ISBN:";
                    return View(db.Livros.Where(x => x.Estado == true).OrderBy(x => x.ISBN).ToPagedList(NumeroPagina, TamanhoPagina));
                }
                if (Ordem == "Título")
                {
                    ViewBag.title = "Ordenado por Título:";
                    return View(db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Titulo).ToPagedList(NumeroPagina, TamanhoPagina));
                }
                if (Ordem == "Autor")
                {
                    ViewBag.title = "Ordenado por Autor:";
                    return View(db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Escreve.Autor.Nome).OrderBy(x => x.Titulo).ToPagedList(NumeroPagina, TamanhoPagina));
                }
                if (Ordem == "Editora")
                {
                    ViewBag.title = "Ordenado por Editora:";
                    return View(db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Editora).ToPagedList(NumeroPagina, TamanhoPagina));
                }
                if (Ordem == "Categoria")
                {
                    ViewBag.title = "Ordenado por Categoria:";
                    return View(db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Pertence.Categoria.Nome).OrderBy(x => x.Titulo).ToPagedList(NumeroPagina, TamanhoPagina));
                }
                if (Ordem == "Data de Adição")
                {
                    ViewBag.title = "Adicionados Recentemente:";
                    return View(db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Adiciona.Data_Adicao).OrderBy(x => x.Titulo).ToPagedList(NumeroPagina, TamanhoPagina));
                }
                else
                {
                    ViewBag.title = "Adicionados Recentemente:";
                    return View(db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Adiciona.Data_Adicao).ToPagedList(NumeroPagina, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
                }
            }
            else
            {
                ViewBag.title = "Adicionados Recentemente:";
                return View(db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Adiciona.Data_Adicao).ToPagedList(NumeroPagina, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
            }
        }

        public ActionResult LivrosPagina(int TamanhoPagina)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            HttpCookie oreo = new HttpCookie("livrosPorPagina");
            oreo.Value = Convert.ToString(TamanhoPagina);    //mudar o tamanho de pagina
            HttpContext.Response.SetCookie(oreo);
            int tamanho = TamanhoPagina;


            if (HttpContext.Request.Cookies["OrdenarPagina"] != null)
            {
                string Ordem = HttpContext.Request.Cookies["OrdenarPagina"].Value;
                if (Ordem == "Data de Edição")
                {
                    ViewBag.title = "Ordenado por Data de Edição:";
                    return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.EdicaoData).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "ISBN")
                {
                    ViewBag.title = "Ordenado por ISBN:";
                    return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.ISBN).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Título")
                {
                    ViewBag.title = "Ordenado por Título:";
                    return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Autor")
                {
                    ViewBag.title = "Ordenado por Autor:";
                    return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Escreve.Autor.Nome).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Editora")
                {
                    ViewBag.title = "Ordenado por Editora:";
                    return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Editora).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Categoria")
                {
                    ViewBag.title = "Ordenado por Categoria:";
                    return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Pertence.Categoria.Nome).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Data de Adição")
                {
                    ViewBag.title = "Adicionados Recentemente:";
                    return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Adiciona.Data_Adicao).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                else
                {
                    ViewBag.title = "Adicionados Recentemente:";
                    return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Adiciona.Data_Adicao).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
                }
            }
            else
            {
                ViewBag.title = "Adicionados Recentemente:";
                return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Adiciona.Data_Adicao).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
            }
        }

        public ActionResult Ordenar(string Ordenar)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            int TamanhoPagina;
            if (HttpContext.Request.Cookies["livrosPorPagina"] != null)
                TamanhoPagina = Convert.ToInt32(HttpContext.Request.Cookies["livrosPorPagina"].Value);
            else
                TamanhoPagina = 20;

            HttpCookie oreo2 = new HttpCookie("OrdenarPagina");
            oreo2.Value = Ordenar;
            HttpContext.Response.SetCookie(oreo2);
            string Ordem = Ordenar;
            if (HttpContext.Request.Cookies["OrdenarPagina"] != null)
            {
                if (Ordem == "Data de Edição")
                {
                    ViewBag.title = "Ordenado por Data de Edição:";
                    return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.EdicaoData).ToPagedList(1, TamanhoPagina));

                }
                if (Ordem == "ISBN")
                {
                    ViewBag.title = "Ordenado por ISBN:";
                    return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.ISBN).ToPagedList(1, TamanhoPagina));

                }
                if (Ordem == "Título")
                {
                    ViewBag.title = "Ordenado por Título:";
                    return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Autor")
                {
                    ViewBag.title = "Ordenado por Autor:";
                    return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Escreve.Autor.Nome).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Editora")
                {
                    ViewBag.title = "Ordenado por Editora:";
                    return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Editora).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Categoria")
                {
                    ViewBag.title = "Ordenado por Categoria:";
                    return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Pertence.Categoria.Nome).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Data de Adição")
                {
                    ViewBag.title = "Adicionados Recentemente:";
                    return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Adiciona.Data_Adicao).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                else
                {
                    ViewBag.title = "Adicionados Recentemente:";
                    return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Adiciona.Data_Adicao).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
                }
            }
            else
            {
                ViewBag.title = "Adicionados Recentemente:";
                return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Adiciona.Data_Adicao).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
            }
        }

        public ActionResult Filtrar(string opcao, string valor)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            List<Livro> filtrada = new List<Livro>();
            int TamanhoPagina;
            if (HttpContext.Request.Cookies["livrosPorPagina"] != null)
                TamanhoPagina = Convert.ToInt32(HttpContext.Request.Cookies["livrosPorPagina"].Value);
            else
                TamanhoPagina = 20;
            //ter em conta caso nao tenha valor
            int flag = 0;
            if (opcao == "Livro")
            {
                flag = 1;
                ViewBag.title = "Foram encontrados " + db.Livros.Where(x => x.Titulo.Contains(valor)).Count() + " resultados para a sua pesquisa:";
                filtrada = db.Livros.Where(x => x.Titulo.Contains(valor)).ToList();
            }
            if (opcao == "Editora")
            {
                flag = 1;
                ViewBag.title = "Foram encontrados " + db.Livros.Where(x => x.Editora.Contains(valor)).Count() + " resultados para a sua pesquisa:";
                filtrada = db.Livros.Where(x => x.Editora.Contains(valor)).ToList();
            }
            if (opcao == "Loja")
            {
                foreach (Disponivel d in db.Disponivels)
                {
                    if (d.Loja.Nome.Contains(valor))
                    {
                        Livro i = db.Livros.Single(x => x.ID_Livro == d.ID_Livro);
                        filtrada.Add(i);
                    }
                }
                flag = 1;
                ViewBag.title = "Foram encontrados " + filtrada.Count() + " resultados para a sua pesquisa:";
            }
            if (opcao == "Filtrar Por:" || valor == "")
            {
                filtrada = db.Livros.ToList();
            }

            if (HttpContext.Request.Cookies["OrdenarPagina"] != null)
            {
                string Ordem = HttpContext.Request.Cookies["OrdenarPagina"].Value;
                if (Ordem == "Data de Edição")
                {
                    if (flag == 0)
                        ViewBag.title = "Ordenado por Data de Edição:";
                    if (modo == "lista")
                        return PartialView("ListLivros", filtrada.Where(x => x.Estado == true).OrderBy(x => x.EdicaoData).ToPagedList(1, TamanhoPagina));
                    else
                        return PartialView("ListLivrosPrincipal", filtrada.Where(x => x.Estado == true).OrderBy(x => x.EdicaoData).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "ISBN")
                {
                    if (flag == 0)
                        ViewBag.title = "Ordenado por ISBN:";
                    if (modo == "lista")
                        return PartialView("ListLivros", filtrada.Where(x => x.Estado == true).OrderBy(x => x.ISBN).ToPagedList(1, TamanhoPagina));
                    else
                        return PartialView("ListLivrosPrincipal", filtrada.Where(x => x.Estado == true).OrderBy(x => x.ISBN).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Título")
                {
                    if (flag == 0)
                        ViewBag.title = "Ordenado por Título:";
                    if (modo == "lista")
                        return PartialView("ListLivros", filtrada.Where(x => x.Estado == true).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                    else
                        return PartialView("ListLivrosPrincipal", filtrada.Where(x => x.Estado == true).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Autor")
                {
                    ViewBag.title = "Ordenado por Autor:";
                    if (modo == "lista")
                        return PartialView("ListLivros", filtrada.Where(x => x.Estado == true).OrderBy(x => x.Escreve.Autor.Nome).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                    else
                        return PartialView("ListLivrosPrincipal", filtrada.Where(x => x.Estado == true).OrderBy(x => x.Escreve.Autor.Nome).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Editora")
                {
                    if (flag == 0)
                        ViewBag.title = "Ordenado por Editora:";
                    if (modo == "lista")
                        return PartialView("ListLivros", filtrada.Where(x => x.Estado == true).OrderBy(x => x.Editora).ToPagedList(1, TamanhoPagina));
                else
                        return PartialView("ListLivrosPrincipal", filtrada.Where(x => x.Estado == true).OrderBy(x => x.Editora).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Data de Adição")
                {
                    if (flag == 0)
                        ViewBag.title = "Adicionados Recentemente:";
                    if (modo == "lista")
                        return PartialView("ListLivros", filtrada.Where(x => x.Estado == true).OrderBy(x => x.Adiciona.Data_Adicao).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                else
                        return PartialView("ListLivrosPrincipal", filtrada.Where(x => x.Estado == true).OrderBy(x => x.Adiciona.Data_Adicao).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                else
                {
                    if (flag == 0)
                        ViewBag.title = "Adicionados Recentemente:";
                    if (modo == "lista")
                        return PartialView("ListLivros", filtrada.Where(x => x.Estado == true).OrderBy(x => x.Adiciona.Data_Adicao).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
                else
                        return PartialView("ListLivrosPrincipal", filtrada.Where(x => x.Estado == true).OrderBy(x => x.Adiciona.Data_Adicao).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
                }
                }
            else
            {
                if (flag == 0)
                    ViewBag.title = "Adicionados Recentemente:";
                if (modo == "lista")
                    return PartialView("ListLivros", filtrada.Where(x => x.Estado == true).OrderBy(x => x.Adiciona.Data_Adicao).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
                else
                    return PartialView("ListLivrosPrincipal", filtrada.Where(x => x.Estado == true).OrderBy(x => x.Adiciona.Data_Adicao).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
            }
        }

        [HttpPost]
        public ActionResult LivroAjax(int id)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            Livro L = db.Livros.Single(x => x.ID_Livro == id);
            ViewBag.CapaLivro = "/Content/Imagens/Livros/" + id + ".jpg";
            return PartialView("VerLivro", L);
        }

        [HttpPost]
        public ActionResult Livro(int id, FormCollection collection)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            try
            {
                Utilizador a = new Utilizador();
                a = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);
                ViewBag.Utilizador = a.ID_Utilizador;
                Leu novo = new Leu();

                novo.ID_Livro = Convert.ToInt32(collection["Livro"]);
                novo.ID_Utilizador = Convert.ToInt32(a);
                novo.Comentario = collection["BotComentar"];
                novo.Data_Comentario = DateTime.Now;

                db.Leus.InsertOnSubmit(novo);
                db.SubmitChanges();
            }
            catch
            {
                ViewBag.Utilizador = null;
            }

            Livro L = db.Livros.Single(x => x.ID_Livro == id);
            ViewBag.CapaLivro = "/Content/Imagens/Livros/" + id + ".jpg";
            ViewBag.Autor = "/Home/Autor/" + L.Escreve.Autor.ID_Autor;


            return View(L);
        }
        [HttpPost]
        public ActionResult Categoria(int id)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            int TamanhoPagina;
            if (HttpContext.Request.Cookies["livrosPorPagina"] != null)
                TamanhoPagina = Convert.ToInt32(HttpContext.Request.Cookies["livrosPorPagina"].Value);
            else
                TamanhoPagina = 20;


            if (HttpContext.Request.Cookies["OrdenarPagina"] != null)
            {
                string Ordem = HttpContext.Request.Cookies["OrdenarPagina"].Value;
                if (Ordem == "Data de Edição")
                {
                    ViewBag.title = "Ordenado por Data de Edição:";
                    if (modo == "lista")
                        return PartialView("ListLivros", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.EdicaoData).ToPagedList(1, TamanhoPagina));
                else
                        return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.EdicaoData).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "ISBN")
                {
                    ViewBag.title = "Ordenado por ISBN:";
                    if (modo == "lista")
                        return PartialView("ListLivros", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.ISBN).ToPagedList(1, TamanhoPagina));
                else
                        return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.ISBN).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Título")
                {
                    ViewBag.title = "Ordenado por Título:";
                    if (modo == "lista")
                        return PartialView("ListLivros", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                else
                        return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.ISBN).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Autor")
                {
                    ViewBag.title = "Ordenado por Autor:";
                    if (modo == "lista")
                        return PartialView("ListLivros", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.Escreve.Autor.Nome).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                else
                        return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.ISBN).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Editora")
                {
                    ViewBag.title = "Ordenado por Editora:";
                    if (modo == "lista")
                        return PartialView("ListLivros", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.Editora).ToPagedList(1, TamanhoPagina));
                    else return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.ISBN).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Categoria")
                {
                    ViewBag.title = "Ordenado por Categoria:";
                    if (modo == "lista")
                        return PartialView("ListLivros", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.Pertence.Categoria.Nome).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                else
                        return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.Pertence.Categoria.Nome).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Data de Adição")
                {
                    ViewBag.title = "Adicionados Recentemente:";
                    if (modo == "lista")
                        return PartialView("ListLivros", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.Adiciona.Data_Adicao).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                else return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.Pertence.Categoria.Nome).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                else
                {
                    ViewBag.title = "Adicionados Recentemente:";
                    if (modo == "lista")
                        return PartialView("ListLivros", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.Adiciona.Data_Adicao).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
                else return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.Pertence.Categoria.Nome).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
            }
            else
            {
                ViewBag.title = "Adicionados Recentemente:";
                if (modo == "lista")
                    return PartialView("ListLivros", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.Adiciona.Data_Adicao).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
            else return PartialView("ListLivrosPrincipal", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.Adiciona.Data_Adicao).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
            }
            }
        [HttpPost]
        public ActionResult Autor(int id)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login", "Account", "");
            }
            Autor a = db.Autors.Single(x => x.ID_Autor == id);

            ViewBag.id = a.ID_Autor;
            ViewBag.Nome = a.Nome;
            ViewBag.Biografia = a.Biografia;
            ViewBag.Pseudonimo = a.Pseudonimo;
            ViewBag.ImagemAutor = "/Content/Imagens/Autores/" + a.ID_Autor + ".jpg";


            int TamanhoPagina;
            if (HttpContext.Request.Cookies["livrosPorPagina"] != null)
                TamanhoPagina = Convert.ToInt32(HttpContext.Request.Cookies["livrosPorPagina"].Value);
            else
                TamanhoPagina = 20;


            if (HttpContext.Request.Cookies["OrdenarPagina"] != null)
            {
                string Ordem = HttpContext.Request.Cookies["OrdenarPagina"].Value;
                if (Ordem == "Data de Edição")
                {
                    ViewBag.title = "Ordenado por Data de Edição:";
                    return PartialView("Autor", db.Livros.Where(x => x.Estado == true).Where(x => x.Escreve.ID_Autor == id).OrderBy(x => x.EdicaoData).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "ISBN")
                {
                    ViewBag.title = "Ordenado por ISBN:";
                    return PartialView("Autor", db.Livros.Where(x => x.Estado == true).Where(x => x.Escreve.ID_Autor == id).OrderBy(x => x.ISBN).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Título")
                {
                    ViewBag.title = "Ordenado por Título:";
                    return PartialView("Autor", db.Livros.Where(x => x.Estado == true).Where(x => x.Escreve.ID_Autor == id).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Autor")
                {
                    ViewBag.title = "Ordenado por Autor:";
                    return PartialView("Autor", db.Livros.Where(x => x.Estado == true).Where(x => x.Escreve.ID_Autor == id).OrderBy(x => x.Escreve.Autor.Nome).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Editora")
                {
                    ViewBag.title = "Ordenado por Editora:";
                    return PartialView("Autor", db.Livros.Where(x => x.Estado == true).Where(x => x.Escreve.ID_Autor == id).OrderBy(x => x.Editora).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Categoria")
                {
                    ViewBag.title = "Ordenado por Categoria:";
                    return PartialView("Autor", db.Livros.Where(x => x.Estado == true).Where(x => x.Escreve.ID_Autor == id).OrderBy(x => x.Pertence.Categoria.Nome).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Data de Adição")
                {
                    ViewBag.title = "Adicionados Recentemente:";
                    return PartialView("Autor", db.Livros.Where(x => x.Estado == true).Where(x => x.Escreve.ID_Autor == id).OrderBy(x => x.Adiciona.Data_Adicao).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                else
                {
                    ViewBag.title = "Adicionados Recentemente:";
                    return PartialView("Autor", db.Livros.Where(x => x.Estado == true).Where(x => x.Escreve.ID_Autor == id).OrderBy(x => x.Adiciona.Data_Adicao).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
                }
            }
            else
            {
                ViewBag.title = "Adicionados Recentemente:";
                return PartialView("Autor", db.Livros.Where(x => x.Estado == true).Where(x => x.Escreve.ID_Autor == id).OrderBy(x => x.Adiciona.Data_Adicao).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
            }
        }
        [HttpPost]
        public void EstadoLivro(int id)
        {
            Livro l = db.Livros.Single(x => x.ID_Livro == id);
            l.Estado = !l.Estado;
            db.SubmitChanges();
        }

        [HttpPost]
        public void Bloquear(int id)
        {
            Utilizador u = db.Utilizadors.Single(x => x.ID_Utilizador == id);
            u.Bloqueado = !u.Bloqueado;
            if (u.Estado == 1) {
                u.Estado = 2;
            }
            if (u.Estado == 2)
            {
                u.Estado = 1;
            }
            db.SubmitChanges();
        }

    }
}