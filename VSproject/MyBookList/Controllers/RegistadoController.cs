using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.IO;

using System.Drawing;
using MyBookList.Models;
using PagedList;
using System.Globalization;

namespace MyBookList.Controllers
{
    [Authorize]
    public class RegistadoController : Controller
    {
        DataClassesMBLDataContext db = new DataClassesMBLDataContext();

        public ActionResult Index(int? pagina)
        {
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
                    return View(db.Livros.Where(x => x.Estado == true).OrderBy(x => x.ID_Livro).ToPagedList(NumeroPagina, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
                }
            }
            else
            {
                ViewBag.title = "Adicionados Recentemente:";
                return View(db.Livros.Where(x => x.Estado == true).OrderBy(x => x.ID_Livro).ToPagedList(NumeroPagina, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
            }
        }

        public ActionResult LivrosPagina(int TamanhoPagina)
        {
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
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.EdicaoData).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "ISBN")
                {
                    ViewBag.title = "Ordenado por ISBN:";
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.ISBN).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Título")
                {
                    ViewBag.title = "Ordenado por Título:";
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Autor")
                {
                    ViewBag.title = "Ordenado por Autor:";
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Escreve.Autor.Nome).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Editora")
                {
                    ViewBag.title = "Ordenado por Editora:";
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Editora).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Categoria")
                {
                    ViewBag.title = "Ordenado por Categoria:";
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Pertence.Categoria.Nome).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Data de Adição")
                {
                    ViewBag.title = "Adicionados Recentemente:";
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Adiciona.Data_Adicao).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                else
                {
                    ViewBag.title = "Adicionados Recentemente:";
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.ID_Livro).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
                }
            }
            else
            {
                ViewBag.title = "Adicionados Recentemente:";
                return PartialView("Livros", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.ID_Livro).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
            }
        }

        public ActionResult Ordenar(string Ordenar)
        {
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
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.EdicaoData).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "ISBN")
                {
                    ViewBag.title = "Ordenado por ISBN:";
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.ISBN).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Título")
                {
                    ViewBag.title = "Ordenado por Título:";
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Autor")
                {
                    ViewBag.title = "Ordenado por Autor:";
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Escreve.Autor.Nome).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Editora")
                {
                    ViewBag.title = "Ordenado por Editora:";
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Editora).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Categoria")
                {
                    ViewBag.title = "Ordenado por Categoria:";
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Pertence.Categoria.Nome).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Data de Adição")
                {
                    ViewBag.title = "Adicionados Recentemente:";
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Adiciona.Data_Adicao).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                else
                {
                    ViewBag.title = "Adicionados Recentemente:";
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.ID_Livro).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
                }
            }
            else
            {
                ViewBag.title = "Adicionados Recentemente:";
                return PartialView("Livros", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.ID_Livro).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
            }
        }

        public ActionResult Filtrar(string opcao, string valor)
        {
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
                    return PartialView("Livros", filtrada.Where(x => x.Estado == true).OrderBy(x => x.EdicaoData).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "ISBN")
                {
                    if (flag == 0)
                        ViewBag.title = "Ordenado por ISBN:";
                    return PartialView("Livros", filtrada.Where(x => x.Estado == true).OrderBy(x => x.ISBN).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Título")
                {
                    if (flag == 0)
                        ViewBag.title = "Ordenado por Título:";
                    return PartialView("Livros", filtrada.Where(x => x.Estado == true).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Autor")
                {
                    ViewBag.title = "Ordenado por Autor:";
                    return PartialView("Livros", filtrada.Where(x => x.Estado == true).OrderBy(x => x.Escreve.Autor.Nome).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Editora")
                {
                    if (flag == 0)
                        ViewBag.title = "Ordenado por Editora:";
                    return PartialView("Livros", filtrada.Where(x => x.Estado == true).OrderBy(x => x.Editora).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Categoria")
                {
                    if (flag == 0)
                        ViewBag.title = "Ordenado por Categoria:";
                    return PartialView("Livros", filtrada.Where(x => x.Estado == true).OrderBy(x => x.Pertence.Categoria.Nome).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Data de Adição")
                {
                    if (flag == 0)
                        ViewBag.title = "Adicionados Recentemente:";
                    return PartialView("Livros", filtrada.Where(x => x.Estado == true).OrderBy(x => x.Adiciona.Data_Adicao).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                else
                {
                    if (flag == 0)
                        ViewBag.title = "Adicionados Recentemente:";
                    return PartialView("Livros", filtrada.Where(x => x.Estado == true).OrderBy(x => x.ID_Livro).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
                }
            }
            else
            {
                if (flag == 0)
                    ViewBag.title = "Adicionados Recentemente:";
                return PartialView("Livros", filtrada.Where(x => x.Estado == true).OrderBy(x => x.ID_Livro).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
            }
        }
        
        public ActionResult LivroA(int id)
        {
            Livro L = db.Livros.Single(x => x.ID_Livro == id);

            Utilizador u = new Utilizador();
            u = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);

            try
            {
                ViewBag.Lido = u.Leus.Single(x => x.ID_Livro == id).Estado;
            }
            catch
            {
                ViewBag.Lido = false;
            }

            try
            {
                ViewBag.Possui = u.Possuis.Single(x => x.ID_Livro == id).Estado;
            }
            catch
            {
                ViewBag.Possui = false;
            }
            ViewBag.CapaLivro = "/Content/Imagens/Livros/" + id + ".jpg";
            ViewBag.Autor = "/Registado/Autor/" + L.Escreve.Autor.ID_Autor;
            return View("LivroAjax");
        }

        public ActionResult LivroAJax(int id)
        {
            Livro L = db.Livros.Single(x => x.ID_Livro == id);
            ViewBag.CapaLivro = "/Content/Imagens/Livros/" + id + ".jpg";
            ViewBag.Autor = "/Home/Autor/" + L.Escreve.Autor.ID_Autor;
            return View(L);
        }

        [HttpPost]
        public ActionResult LivroAjax(FormCollection collection)
        {
            try
            {
                Utilizador a = new Utilizador();
                a = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);
                ViewBag.Utilizador = a.ID_Utilizador;
                Leu novo = new Leu();

                novo = db.Leus.Single(x => x.ID_Livro == Convert.ToInt32(collection["Livro"]) && x.ID_Utilizador == a.ID_Utilizador);

                novo.Comentario = collection["BotComentar"];
                novo.Data_Comentario = DateTime.Now;

                db.SubmitChanges();
            }
            catch
            {
                ViewBag.Utilizador = null;
            }

            Livro L = db.Livros.Single(x => x.ID_Livro == Convert.ToInt32(collection["Livro"]));
            ViewBag.CapaLivro = "/Content/Imagens/Livros/" + L.ID_Livro + ".jpg";
            ViewBag.Autor = "/Home/Autor/" + L.Escreve.Autor.ID_Autor;


            return RedirectToAction("LivroAjax","Registado", L.ID_Livro);
        }

        [HttpPost]
        public ActionResult Categoria(int id)
        {
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
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.EdicaoData).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "ISBN")
                {
                    ViewBag.title = "Ordenado por ISBN:";
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.ISBN).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Título")
                {
                    ViewBag.title = "Ordenado por Título:";
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Autor")
                {
                    ViewBag.title = "Ordenado por Autor:";
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.Escreve.Autor.Nome).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Editora")
                {
                    ViewBag.title = "Ordenado por Editora:";
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.Editora).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Categoria")
                {
                    ViewBag.title = "Ordenado por Categoria:";
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.Pertence.Categoria.Nome).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Data de Adição")
                {
                    ViewBag.title = "Adicionados Recentemente:";
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.Adiciona.Data_Adicao).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                else
                {
                    ViewBag.title = "Adicionados Recentemente:";
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.ID_Livro).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
                }
            }
            else
            {
                ViewBag.title = "Adicionados Recentemente:";
                return PartialView("Livros", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.ID_Livro).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
            }
        }

        [HttpPost]
        public ActionResult Autor(int id)
        {
            ViewBag.Nome = db.Autors.Single(x => x.ID_Autor == id).Nome;
            ViewBag.Biografia = db.Autors.Single(x => x.ID_Autor == id).Biografia;
            ViewBag.Pseudonimo = db.Autors.Single(x => x.ID_Autor == id).Pseudonimo;

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
                    return PartialView(db.Livros.Where(x => x.Estado == true).Where(x => x.Escreve.ID_Autor == id).OrderBy(x => x.EdicaoData).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "ISBN")
                {
                    ViewBag.title = "Ordenado por ISBN:";
                    return PartialView(db.Livros.Where(x => x.Estado == true).Where(x => x.Escreve.ID_Autor == id).OrderBy(x => x.ISBN).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Título")
                {
                    ViewBag.title = "Ordenado por Título:";
                    return PartialView(db.Livros.Where(x => x.Estado == true).Where(x => x.Escreve.ID_Autor == id).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Autor")
                {
                    ViewBag.title = "Ordenado por Autor:";
                    return PartialView(db.Livros.Where(x => x.Estado == true).Where(x => x.Escreve.ID_Autor == id).OrderBy(x => x.Escreve.Autor.Nome).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Editora")
                {
                    ViewBag.title = "Ordenado por Editora:";
                    return PartialView(db.Livros.Where(x => x.Estado == true).Where(x => x.Escreve.ID_Autor == id).OrderBy(x => x.Editora).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Categoria")
                {
                    ViewBag.title = "Ordenado por Categoria:";
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).Where(x => x.Escreve.ID_Autor == id).OrderBy(x => x.Pertence.Categoria.Nome).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                if (Ordem == "Data de Adição")
                {
                    ViewBag.title = "Adicionados Recentemente:";
                    return PartialView(db.Livros.Where(x => x.Estado == true).Where(x => x.Escreve.ID_Autor == id).OrderBy(x => x.Adiciona.Data_Adicao).OrderBy(x => x.Titulo).ToPagedList(1, TamanhoPagina));
                }
                else
                {
                    ViewBag.title = "Adicionados Recentemente:";
                    return PartialView(db.Livros.Where(x => x.Estado == true).Where(x => x.Escreve.ID_Autor == id).OrderBy(x => x.ID_Livro).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
                }
            }
            else
            {
                ViewBag.title = "Adicionados Recentemente:";
                return PartialView(db.Livros.Where(x => x.Estado == true).Where(x => x.Escreve.ID_Autor == id).OrderBy(x => x.ID_Livro).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
            }
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Perfil()
        {
            Utilizador a = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);

            //verificar se user ja tem foto
            string caminhoFoto1 = HttpContext.Server.MapPath("/Content/Imagens/Users/") + a.ID_Utilizador + ".jpg";
            string caminhoFoto2 = HttpContext.Server.MapPath("/Content/Imagens/Users/") + a.ID_Utilizador + ".jpeg";
            string caminhoFoto3 = HttpContext.Server.MapPath("/Content/Imagens/Users/") + a.ID_Utilizador + ".png";

            if (System.IO.File.Exists(caminhoFoto1))
            {
                ViewBag.path = a.ID_Utilizador + ".jpg";
            }
            else
            {
                if (System.IO.File.Exists(caminhoFoto2))
                {
                    ViewBag.path = a.ID_Utilizador + ".jpeg";
                }
                else
                {
                    if (System.IO.File.Exists(caminhoFoto3))
                    {
                        ViewBag.path = a.ID_Utilizador + ".png";
                    }
                    else
                    {
                        ViewBag.path = "defaultUser.png";
                    }
                }
            }

            //obter FeedBack do utilizador

            return View(db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name));
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Editar(int id)
        {
            Utilizador a = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);

            //verificar se user ja tem foto
            string caminhoFoto1 = HttpContext.Server.MapPath("/Content/Imagens/Users/") + a.ID_Utilizador + ".jpg";
            string caminhoFoto2 = HttpContext.Server.MapPath("/Content/Imagens/Users/") + a.ID_Utilizador + ".jpeg";
            string caminhoFoto3 = HttpContext.Server.MapPath("/Content/Imagens/Users/") + a.ID_Utilizador + ".png";

            if (System.IO.File.Exists(caminhoFoto1))
            {
                ViewBag.path = a.ID_Utilizador + ".jpg";
            }
            else
            {
                if (System.IO.File.Exists(caminhoFoto2))
                {
                    ViewBag.path = a.ID_Utilizador + ".jpeg";
                }
                else
                {
                    if (System.IO.File.Exists(caminhoFoto3))
                    {
                        ViewBag.path = a.ID_Utilizador + ".png";
                    }
                    else
                    {
                        ViewBag.path = "defaultUser.png";
                    }
                }
            }

            return View(db.Utilizadors.Single(x => x.ID_Utilizador == id));
        }

        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Editar(FormCollection collection)
        {
            Utilizador u = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);

            if (string.IsNullOrEmpty(collection["Nome"]))
                ModelState.AddModelError("Nome", "Tem de preencher o campo!");

            //É para deixar alterar o username e o Email?

            if (string.IsNullOrEmpty(collection["Telefone"]))
                ModelState.AddModelError("Telefone", "Tem de preencher o campo!");

            if (string.IsNullOrEmpty(collection["Data"]))
                ModelState.AddModelError("Data", "Tem de preencher o campo!");
            else
            {
                DateTime d = Convert.ToDateTime(collection["Data"]);
                if (d >= DateTime.Now)
                    ModelState.AddModelError("Data", "Data inválida");
            }

            if (string.IsNullOrEmpty(collection["Localidade"]))
                ModelState.AddModelError("Localidade", "Tem de preencher o campo!");

            if (string.IsNullOrEmpty(collection["Rua"]))
                ModelState.AddModelError("Rua", "Tem de preencher o campo!");

            if (string.IsNullOrEmpty(collection["Porta"]))
                ModelState.AddModelError("Porta", "Tem de preencher o campo!");

            if (string.IsNullOrEmpty(collection["CodigoPostal"]))
                ModelState.AddModelError("CodigoPostal", "Tem de preencher o campo!");

            if (ModelState.IsValid)
            {
                u.Nome = collection["Nome"];
                u.ContactoTelefone = collection["Telefone"];
                u.Data_Nascimento = Convert.ToDateTime(collection["Data"]);
                u.MoradaLocalidade = collection["Localidade"];
                u.MoradaRua = collection["Rua"];
                u.MoradaPorta = collection["Porta"];
                u.MoradaCodigo_Postal = collection["CodigoPostal"];
                u.Estado = 1;

                db.SubmitChanges();

                return RedirectToAction("Perfil", "Registado");
            }
            else
            {
                return View(u);
            }
        }

        [HttpPost]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Foto(HttpPostedFileBase foto)
        {
            Utilizador a = new Utilizador();
            a = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);

            if (foto == null)
            {
                ModelState.AddModelError("foto", "Tem que adicionar um ficheiro!");
                return RedirectToAction("Editar",a.ID_Utilizador);
            }

            if ((foto.ContentType != "image/jpg") && (foto.ContentType != "image/png") && (foto.ContentType != "image/jpeg"))
            {
                ModelState.AddModelError("foto", "Formato de ficheiro invalido! Apenas JPG, JPEG e PNG são permitidos.");
            }

            if(ModelState.IsValid)
            {
                //verificar se user ja tem foto
                string caminhoFoto1 = HttpContext.Server.MapPath("/Content/Imagens/Users/") + a.ID_Utilizador + ".jpg";
                string caminhoFoto2 = HttpContext.Server.MapPath("/Content/Imagens/Users/") + a.ID_Utilizador + ".jpeg";
                string caminhoFoto3 = HttpContext.Server.MapPath("/Content/Imagens/Users/") + a.ID_Utilizador + ".png";

                //Apagar qualquer foto antiga do mesmo user!
                if (System.IO.File.Exists(caminhoFoto1))
                {
                    System.IO.File.Delete(caminhoFoto1);
                }
                if (System.IO.File.Exists(caminhoFoto2))
                {
                    System.IO.File.Delete(caminhoFoto2);
                }
                if (System.IO.File.Exists(caminhoFoto3))
                {
                    System.IO.File.Delete(caminhoFoto3);
                }

                // constroi o caminho para a pasta onde gauda o ficheiro
                string caminho = HttpContext.Server.MapPath("/Content/Imagens/Users");

                //retira extensão do ficheiro
                string fileExtension = Path.GetExtension(foto.FileName).ToLower();

                //Guardar a foto na pasta
                foto.SaveAs(caminho + "/" + a.ID_Utilizador + fileExtension);

                //HttpCookie cookie = new HttpCookie("Foto");
                //cookie.Value = caminho + "/" + a.ID_Utilizador + fileExtension;
                //Response.Cookies.Add(cookie);

                return RedirectToAction("Perfil");
            }
            else
            {
                return RedirectToAction("Perfil");
            }
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Biblioteca()
        {
            // encontrar o utilizador
            Utilizador a = new Utilizador();
            a = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);

            //lista dos livros lidos
            List<Leu> lidos = new List<Leu>();
            foreach (var item in a.Leus)
            {
                //apenas passar os livros com estado válido
                if(item.Estado == true)
                    lidos.Add(item);
            } 

            ViewBag.lidos = lidos;


            //lista dos livros que possui
            List<Possui> possui = new List<Possui>();
            foreach (var item in a.Possuis)
            {
                if (item.Estado == true)
                {
                    possui.Add(item);
                }
            }
            ViewBag.biblioteca = possui;



            return View(db.Livros);
        }

        //metodo para alterar a visibilidade(disponibilidade para emprestimo) do livro que possui
        [HttpPost]
        public void visibilidadeAlterada(int id)
        {
            // encontrar o utilizador
            Utilizador a = new Utilizador();
            a = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);

            //encontrar o livro
            Possui p = new Possui();
            try
            {
                p = a.Possuis.Single(x => x.ID_Livro == id);
                p.Visibilidade = !p.Visibilidade;
            }
            catch
            {
                p.ID_Utilizador = a.ID_Utilizador;
                p.ID_Livro = id;
                p.Estado = true;
                p.Data = DateTime.Now;
                p.Visibilidade = true;
                db.Possuis.InsertOnSubmit(p);
            }

            db.SubmitChanges();

            return;
        }

        public ActionResult livroLido(int id)
        {
            // encontrar o utilizador
            Utilizador a = new Utilizador();
            a = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);

            try
            {
                //verificar se o utilizador possui o livro
                Possui p = new Possui();
                p = a.Possuis.Single(x => x.ID_Livro == id);

                //se possui, alterar o valor do atributo lido para o inverso
                p.Lido = !p.Lido;
            }
            catch { }
            finally
            {//alterar o valor na entidade lido
                try
                {//verificar se a relação já existe
                    Leu L = new Leu();
                    L = a.Leus.Single(x => x.ID_Livro == id);

                    L.Estado = !L.Estado;
                }
                catch
                {//se a relação nao existir necessário criar
                    Leu L = new Leu();
                    L.ID_Utilizador = a.ID_Utilizador;
                    L.ID_Livro = id;
                    L.Estado = true;
                    db.Leus.InsertOnSubmit(L);
                }

                db.SubmitChanges();
            }

            List<Leu> lidos = new List<Leu>();
            foreach (var item in a.Leus)
            {
                //apenas passar os livros com estado válido
                if (item.Estado == true)
                    lidos.Add(item);
            }

            ViewBag.lidos = lidos;
            return PartialView("livroLido", db.Livros);
        }

        [HttpPost]
        public ActionResult ApagarLivro(string id)
        {
            int LivroID = Convert.ToInt32(id);

            //encontrar o user
            Utilizador a = new Utilizador();
            a = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);

            Possui P = db.Possuis.Single(x => x.ID_Livro == LivroID && x.ID_Utilizador == a.ID_Utilizador);
            P.Estado = false;

            db.SubmitChanges();

            //lista dos livros que possui
            List<Possui> possui = new List<Possui>();
            foreach (var item in a.Possuis)
            {
                if (item.Estado == true)
                {
                    possui.Add(item);
                }
            }
            ViewBag.biblioteca = possui;


            return PartialView("ApagarLivro", db.Livros);
        }


        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Emprestimos()
        {
            //encontrar o user
            Utilizador a = new Utilizador();
            a = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);

            return View(db.Emprestas.Where(x => x.ID_UserRecebe == a.ID_Utilizador && x.Data_Devolucao == null).OrderBy(x => x.Data_Emprestimo));
        }

        public ActionResult EmprestimosDecorrer()
        {
            //encontrar o user
            Utilizador a = new Utilizador();
            a = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);

            return PartialView("EmprestimosDecorrer", db.Emprestas.Where(x => x.ID_UserRecebe == a.ID_Utilizador && x.Data_Devolucao == null).OrderBy(x => x.Data_Emprestimo));
        }

        public ActionResult EmprestimosTerminados()
        {
            Utilizador a = new Utilizador();
            a = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);

            List<Empresta> lista = db.Emprestas.Where(x => x.ID_UserRecebe == a.ID_Utilizador && x.Data_Devolucao != null).ToList();

            //foreach (var item in db.Emprestas.Where(x => x.ID_UserEmpresta == a.ID_Utilizador && x.Data_Devolucao != null))
            //    lista.Add(item);

            return PartialView("EmprestimosTerminados", lista.OrderByDescending(x=> x.Data_Devolucao)); ;
        }

        public ActionResult PedidosEfetuados()
        {
            Utilizador a = new Utilizador();
            a = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);

            return PartialView("PedidosEfetuados", db.Pedes.Where(x => x.ID_Utilizador == a.ID_Utilizador && x.Estado_Pedido == false).OrderBy(x=>x.Data_Criacao));
        }

        public ActionResult PedidosRecebidos()
        {
            Utilizador a = new Utilizador();
            a = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);

            ////Pedidos de empréstimo recebidos
            List<Pede> R = new List<Pede>();
           
            foreach(var item in a.Possuis)
            {
                foreach (var pedido in db.Pedes.Where(x => x.Estado_Pedido == false && x.ID_Livro == item.ID_Livro))
                {
                    if(item.Visibilidade == true) R.Add(pedido);
                }
                    
            }

            return PartialView("PedidosRecebidos", R.OrderBy(x=>x.Data_Criacao));
        }

        public ActionResult LivrosEmprestados()
        {
            Utilizador a = new Utilizador();
            a = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);

            return PartialView("LivrosEmprestados", db.Emprestas.Where(x => x.ID_UserEmpresta == a.ID_Utilizador && x.Data_Devolucao == null).OrderBy(x=>x.Data_Emprestimo));
        }

        public ActionResult CancelarPedido(int id)
        {
            Utilizador a = new Utilizador();
            a = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);
            Pede P = new Pede();
            try
            {
                P = db.Pedes.Single(x => x.ID_Utilizador == a.ID_Utilizador && x.ID_Livro == id && x.Estado_Pedido == false);
                P.Estado_Pedido = true;
                db.SubmitChanges();
            }
            catch
            {
                //alguma coisa correu mal
            }

            return RedirectToAction("Emprestimos");
        }

        public ActionResult DarFeedBack()
        {
            return View();
        }

        public ActionResult FeedBackUser(int id)
        {
            Utilizador u = db.Utilizadors.Single(x => x.ID_Utilizador == id);

            List<Empresta> lista = db.Emprestas.Where(x => x.ID_UserRecebe == id && x.Data_Devolucao != null).ToList();
            double FR = 0;
            foreach (var item in lista)
                FR = FR + Convert.ToDouble(item.ValueFeedBack_userEmpresta);

            FR = FR / lista.Count();

            ViewBag.NRecebidos = lista.Count();
            ViewBag.FR = FR;

            lista.Clear();

            lista = db.Emprestas.Where(x => x.ID_UserEmpresta == id && x.Data_Devolucao != null).ToList();

            FR = 0;
            foreach (var item in lista)
                FR = FR + Convert.ToDouble(item.ValueFeedBack_userRecebe);

            FR = FR / lista.Count();
            ViewBag.NEmprestados = lista.Count();
            ViewBag.FE = FR;

            return View(u);
        }

        //User que recebe, nao atualiza data de devolução
        [HttpPost]
        public ActionResult DarFeedBack(FormCollection collection)
        {
            Empresta e = new Empresta();
            try
            {
                int livroID = Convert.ToInt32(collection["livro"]);
                int emprestaID = Convert.ToInt32(collection["empresta"]);
                int recebeID = Convert.ToInt32(collection["recebe"]);

                e = (db.Emprestas.Single(x => x.ID_Livro == livroID && x.ID_UserEmpresta == emprestaID && x.ID_UserRecebe == recebeID));

                if (String.IsNullOrEmpty(collection["Feedback"]) && !String.IsNullOrEmpty(collection["Comment"]))
                    ModelState.AddModelError("Feedback", "Tem de introduzir um valor!");

                if (collection["Comment"].Length > 300)
                {
                    ModelState.AddModelError("Comment", "Não pode exceder 300 caracteres!");
                }

                //se nao houver erros guardar o FeedBack e o comentario
                if (ModelState.IsValid)
                {
                    Utilizador u = db.Utilizadors.Single(x => x.ID_Utilizador == e.ID_UserEmpresta);

                    if (u.Feedback == null)
                    {
                        e.ValueFeedBack_userRecebe = Convert.ToDouble(collection["Feedback"]);

                        //se o comentario nao for nulo guarda
                        if (!string.IsNullOrEmpty(collection["Comment"])) e.FeedBackComment_UserRecebe = collection["Comment"];

                        //atualizar Feedback
                        u.Feedback = e.ValueFeedBack_userRecebe;
                    }
                    else
                    {
                        u.Feedback = u.Feedback * 2;
                        u.Feedback = u.Feedback - e.ValueFeedBack_userRecebe;

                        e.ValueFeedBack_userRecebe = Convert.ToDouble(collection["Feedback"]);
                        //se o comentario nao for nulo guarda
                        if (!string.IsNullOrEmpty(collection["Comment"])) e.FeedBackComment_UserRecebe = collection["Comment"];

                        u.Feedback = u.Feedback + e.ValueFeedBack_userRecebe;
                        u.Feedback = u.Feedback / 2;

                    }

                    db.SubmitChanges();

                    return RedirectToAction("Emprestimos");
                }

            }
            catch { } 
            
            return View(e);
        }


        [HttpPost]
        public ActionResult VisualizarPedido(FormCollection dados)
        {
            Pede P = new Pede();

            //encontrar pedido
            try
            {
                int livroID = Convert.ToInt32(dados["livro"]);
                int userID = Convert.ToInt32(dados["user"]);
                DateTime data = Convert.ToDateTime(dados["data"]);

                P = (db.Pedes.Single(x=>x.ID_Livro == livroID && x.ID_Utilizador == userID && x.Data_Criacao == data));

            }
            catch { }

            return View(P);
        }


        [HttpPost]
        public ActionResult AceitarPedido(FormCollection dados)
        {
            Pede P = new Pede();
            Empresta E = new Empresta();

            //encontrar o user que vai emprestar
            Utilizador a = new Utilizador();
            a = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);

            //encontrar pedido
            try
            {
                int livroID = Convert.ToInt32(dados["livro"]);
                int userID = Convert.ToInt32(dados["user"]);
                DateTime data = Convert.ToDateTime(dados["data"]);

                P = (db.Pedes.Single(x => x.ID_Livro == livroID && x.ID_Utilizador == userID && x.Data_Criacao == data));

            }
            catch { }

            //criar o emprestimo
            E.ID_Livro = P.ID_Livro;
            E.ID_UserEmpresta = a.ID_Utilizador;
            E.ID_UserRecebe = P.ID_Utilizador;
            E.Data_Emprestimo = DateTime.Now;

            //mudar o estado do pedido
            P.Estado_Pedido = true;

            //guardar alterações
            db.Emprestas.InsertOnSubmit(E);
            db.SubmitChanges();

            return RedirectToAction("Emprestimos");
        }


        //User que empresta
        [HttpPost]
        public ActionResult LivroDevolvido(FormCollection dados)
        {
            //encontrar o user que empresta
            Utilizador a = new Utilizador();
            a = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);

            Empresta e = new Empresta();

            //encontrar empréstimo
            try
            {
                int livroID = Convert.ToInt32(dados["livro"]);
                DateTime data = Convert.ToDateTime(dados["data"]);
                int recebeID = Convert.ToInt32(dados["user"]);

                e = (db.Emprestas.Single(x => x.ID_Livro == livroID && x.ID_UserEmpresta == a.ID_Utilizador && x.ID_UserRecebe == recebeID &&
                                         x.Data_Emprestimo.Year == data.Year && x.Data_Emprestimo.Month == data.Month &&
                                         x.Data_Emprestimo.Day == data.Day && x.Data_Emprestimo.Hour == data.Hour &&
                                         x.Data_Emprestimo.Minute == data.Minute && x.Data_Emprestimo.Second == data.Second));

                //verificar o modelstate
                if (string.IsNullOrEmpty(dados["FeedBack"]))
                {
                    ModelState.AddModelError("", "");
                }

                if (dados["Comment"].Length > 300)
                {
                    ModelState.AddModelError("Comment", "Não pode exceder 300 caracteres!");
                }

                //se nao houver erros guardar o FeedBack e o comentario
                if (ModelState.IsValid)
                {
                    e.ValueFeedBack_userEmpresta = Convert.ToDouble(dados["FeedBack"]);

                    //se o comentario nao for nulo guarda
                    if (!string.IsNullOrEmpty(dados["Comment"])) e.FeedBackComment_UserEmpresta = dados["Comment"];

                    //data
                    e.Data_Devolucao = DateTime.Now;


                    //atualizar FeedBack user que recebe
                    Utilizador u = db.Utilizadors.Single(x => x.ID_Utilizador == e.ID_UserRecebe);
                    if (u.Feedback==null)
                    {
                        u.Feedback = e.ValueFeedBack_userEmpresta;
                    }
                    else
                    {
                        u.Feedback = u.Feedback + e.ValueFeedBack_userEmpresta;
                        u.Feedback = u.Feedback / 2;

                    }


                    db.SubmitChanges();

                    return RedirectToAction("Emprestimos");
                }

            }
            catch { }

            return View(e);

        }


        [HttpPost]
        public ActionResult Problemas(FormCollection dados)
        {
            //encontrar o user que empresta
            Utilizador a = new Utilizador();
            a = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);

            Empresta e = new Empresta();

            //encontrar empréstimo
            try
            {
                int livroID = Convert.ToInt32(dados["livro"]);
                DateTime data = Convert.ToDateTime(dados["data"]);
                int recebeID = Convert.ToInt32(dados["user"]);

                e = (db.Emprestas.Single(x => x.ID_Livro == livroID && x.ID_UserEmpresta == a.ID_Utilizador && x.ID_UserRecebe == recebeID &&
                                         x.Data_Emprestimo.Year == data.Year && x.Data_Emprestimo.Month == data.Month &&
                                         x.Data_Emprestimo.Day == data.Day && x.Data_Emprestimo.Hour == data.Hour &&
                                         x.Data_Emprestimo.Minute == data.Minute && x.Data_Emprestimo.Second == data.Second));

                //verificar o modelstate
                if (string.IsNullOrEmpty(dados["Comment"]))
                {
                    ModelState.AddModelError("", "");
                }

                if (dados["Comment"].Length > 300)
                {
                    ModelState.AddModelError("Comment", "Não pode exceder 300 caracteres!");
                }

                //se nao houver erros guardar o FeedBack (será 0) e o comentario(será descrição do problema)
                if (ModelState.IsValid)
                {
                    //guardar FeedBack
                    e.ValueFeedBack_userEmpresta = 0;

                    //guardar comentário
                    e.FeedBackComment_UserEmpresta = dados["Comment"];

                    //data
                    e.Data_Devolucao = DateTime.Now;


                    //atualizar FeedBack user que recebe
                    Utilizador u = db.Utilizadors.Single(x => x.ID_Utilizador == e.ID_UserRecebe);

                    u.Feedback = u.Feedback + e.ValueFeedBack_userEmpresta;
                    u.Feedback = u.Feedback / 2;


                    db.SubmitChanges();

                    return RedirectToAction("Emprestimos");
                }

            }
            catch { }

            return View(e);
        }


        public ActionResult PedirEmprestado(int id)
        {
            Utilizador u = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);

            if (u.MoradaCodigo_Postal == null || u.MoradaLocalidade == null || u.MoradaRua == null || u.MoradaPorta == null || u.ContactoTelefone == null)
                return View();
            else
            {
                Pede P = new Pede();
                
                if(!u.Possuis.Any(x => x.ID_Livro == id)){
                    P.ID_Livro = id;
                    P.ID_Utilizador = u.ID_Utilizador;
                    P.Estado_Pedido = false;
                    P.Data_Criacao = DateTime.Now;
                    db.Pedes.InsertOnSubmit(P);
                }
                
                db.SubmitChanges();

                return RedirectToAction("Index","Registado","");
            }
        }


    }
}