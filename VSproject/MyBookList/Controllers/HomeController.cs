using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using MyBookList.Models;

namespace MyBookList.Controllers
{
    public class HomeController : Controller
    {
        DataClassesMBLDataContext db = new DataClassesMBLDataContext();

        public ActionResult Index(int? pagina)
        {
            if (HttpContext.User.Identity.Name != "")
                return RedirectToAction("Index", "Registado");
            else
                if (Session["admin"] != null)
                return RedirectToAction("Home", "BackOffice");

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
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Adiciona.Data_Adicao).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
                }
            }
            else
            {
                ViewBag.title = "Adicionados Recentemente:";
                return PartialView("Livros", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Adiciona.Data_Adicao).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
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
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Adiciona.Data_Adicao).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
                }
            }
            else
            {
                ViewBag.title = "Adicionados Recentemente:";
                return PartialView("Livros", db.Livros.Where(x => x.Estado == true).OrderBy(x => x.Adiciona.Data_Adicao).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
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
                    return PartialView("Livros", filtrada.Where(x => x.Estado == true).OrderBy(x => x.Adiciona.Data_Adicao).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
                }
            }
            else
            {
                if (flag == 0)
                    ViewBag.title = "Adicionados Recentemente:";
                return PartialView("Livros", filtrada.Where(x => x.Estado == true).OrderBy(x => x.Adiciona.Data_Adicao).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
            }
        }

        [HttpPost]
        public ActionResult LivroAjax(int id)
        {
            Livro L = db.Livros.Single(x => x.ID_Livro == id);

            Utilizador u = new Utilizador();
            

            try
            {
                u = db.Utilizadors.Single(x => x.Username == HttpContext.User.Identity.Name);
                ViewBag.ID = u.ID_Utilizador;
                ViewBag.Lido = u.Leus.Single(x => x.ID_Livro == id).Estado;
            }
            catch
            {
                ViewBag.Lido = false;
                ViewBag.ID = null;
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
            ViewBag.Autor = "/Home/Autor/" + L.Escreve.Autor.ID_Autor;
            return PartialView(L);
        }

        public ActionResult Livro(FormCollection collection)
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


            return View(L);
        }

        [HttpPost]
        public ActionResult Livro(int id, FormCollection collection)
        {
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
                    return PartialView("Livros", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.Adiciona.Data_Adicao).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
                }
            }
            else
            {
                ViewBag.title = "Adicionados Recentemente:";
                return PartialView("Livros", db.Livros.Where(x => x.Estado == true).Where(x => x.Pertence.ID_Categoria == id).OrderBy(x => x.Adiciona.Data_Adicao).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
            }
        }

        [HttpPost]
        public ActionResult Autor(int id)
        {
            Autor a = db.Autors.Single(x => x.ID_Autor == id);

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
                    return PartialView("Autor",db.Livros.Where(x => x.Estado == true).Where(x => x.Escreve.ID_Autor == id).OrderBy(x => x.EdicaoData).ToPagedList(1, TamanhoPagina));
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
                return PartialView(db.Livros.Where(x => x.Estado == true).Where(x => x.Escreve.ID_Autor == id).OrderBy(x => x.Adiciona.Data_Adicao).ToPagedList(1, TamanhoPagina)); //MUDAR QUANDO HOUVER DATAS DE EDIÇÃO
            }
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Termos()
        {
            ViewBag.Message = "Termos e Condições";

            return View();
        }
    }
}