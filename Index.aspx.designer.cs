using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Zadanie_1
{
    public partial class Index : System.Web.UI.Page
    {
        string lokalizacjaXML = HttpContext.Current.Server.MapPath("~/xml/Telefonia.xml");
        string lokalizacjaBIN = HttpContext.Current.Server.MapPath("~/binary/id.bin");


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ZaladujTabele();
            }
        }

        private void ZaladujTabele()
        {
            if (ViewState["DaneWidoczne"] != null)
            {
                GridTabela.DataSource = ViewState["DaneWidoczne"];
            }
            else
            {
                DataSet tempDane = new DataSet();
                tempDane.ReadXml(lokalizacjaXML);

                DataTable tabela;

                if (tempDane.Tables.Count > 0)
                {
                    tabela = tempDane.Tables[0];

                    if (!tabela.Columns.Contains("id"))
                        tabela.Columns.Add("id", typeof(string));

                    XmlDocument dokumentXml = new XmlDocument();
                    dokumentXml.Load(lokalizacjaXML);
                    var nodes = dokumentXml.SelectNodes("//subscriber");

                    for (int i = 0; i < nodes.Count && i < tabela.Rows.Count; i++)
                    {
                        var atrybut = nodes[i].Attributes["id"];
                        if (atrybut != null)
                            tabela.Rows[i]["id"] = atrybut.Value;
                    }
                }
                else
                {
                    tabela = new DataTable();
                    tabela.Columns.Add("id");
                    tabela.Columns.Add("name");
                    tabela.Columns.Add("surname");
                    tabela.Columns.Add("city");
                    tabela.Columns.Add("phoneNumber");
                }

                GridTabela.DataSource = tabela;
            }

            GridTabela.DataKeyNames = new string[] { "id" };
            GridTabela.DataBind();
        }

        protected void GridTabela_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridTabela.EditIndex = e.NewEditIndex;
            ZaladujTabele();
        }

        protected void GridTabela_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridTabela.EditIndex = -1;
            ZaladujTabele();
        }

        protected void GridTabela_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string id = GridTabela.DataKeys[e.RowIndex].Value.ToString();

            GridViewRow rzad = GridTabela.Rows[e.RowIndex];
            string newName = ((TextBox)rzad.Cells[0].Controls[0]).Text;
            string newSurname = ((TextBox)rzad.Cells[1].Controls[0]).Text;
            string newCity = ((TextBox)rzad.Cells[2].Controls[0]).Text;
            string newPhone = ((TextBox)rzad.Cells[3].Controls[0]).Text;

            XmlDocument dokumentXml = new XmlDocument();
            dokumentXml.Load(lokalizacjaXML);

            XmlNode rekord = dokumentXml.SelectSingleNode($"//subscriber[@id='{id}']");
            if (rekord != null)
            {
                rekord.SelectSingleNode("name").InnerText = newName;
                rekord.SelectSingleNode("surname").InnerText = newSurname;
                rekord.SelectSingleNode("city").InnerText = newCity;
                rekord.SelectSingleNode("phoneNumber").InnerText = newPhone;

                dokumentXml.Save(lokalizacjaXML);
            }

            GridTabela.EditIndex = -1;
            ViewState.Remove("DaneWidoczne");
            ZaladujTabele();
        }

        protected void GridTabela_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string id = GridTabela.DataKeys[e.RowIndex].Value.ToString();
            XmlDocument dokumentXml = new XmlDocument();
            dokumentXml.Load(lokalizacjaXML);

            XmlNode rekord = dokumentXml.SelectSingleNode($"//subscriber[@id='{id}']");
            if (rekord != null)
            {
                dokumentXml.DocumentElement.RemoveChild(rekord);
                dokumentXml.Save(lokalizacjaXML);
            }

            ViewState.Remove("DaneWidoczne");
            ZaladujTabele();
        }

        protected void GridTabela_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (Control kontrolka in e.Row.Cells[e.Row.Cells.Count - 1].Controls)
                {
                    if (kontrolka is LinkButton btn)
                    {
                        switch (btn.CommandName)
                        {
                            case "Delete":
                                btn.CssClass = "grid-przycisk usun-button";
                                btn.OnClientClick = "return confirm('Czy na pewno chcesz usunąć ten rekord?');";
                                break;
                            case "Edit":
                                btn.CssClass = "grid-przycisk edytuj-button";
                                break;
                            case "Update":
                                btn.CssClass = "grid-przycisk zapisz-button";
                                break;
                            case "Cancel":
                                btn.CssClass = "grid-przycisk anuluj-button";
                                break;
                        }
                    }
                }
            }
        }

        protected void BtnSortuj_Click(object sender, EventArgs e)
        {
            string pole = DropSortujPole.SelectedValue;
            string kierunek = DropKierunek.SelectedValue;

            XmlDocument dokumentXml = new XmlDocument();
            dokumentXml.Load(lokalizacjaXML);
            var rekord = dokumentXml.SelectNodes("//subscriber");

            DataTable posortowane = new DataTable();
            posortowane.Columns.Add("id");
            posortowane.Columns.Add("name");
            posortowane.Columns.Add("surname");
            posortowane.Columns.Add("city");
            posortowane.Columns.Add("phoneNumber");

            var lista = rekord.Cast<XmlNode>()
                .Select(nowy => new
                {
                    id = nowy.Attributes["id"]?.Value ?? "",
                    name = nowy["name"]?.InnerText ?? "",
                    surname = nowy["surname"]?.InnerText ?? "",
                    city = nowy["city"]?.InnerText ?? "",
                    phoneNumber = nowy["phoneNumber"]?.InnerText ?? ""
                })
                .ToList();

            lista = kierunek == "asc"
                ? lista.OrderBy(x => GetWartosc(x, pole)).ToList()
                : lista.OrderByDescending(x => GetWartosc(x, pole)).ToList();

            foreach (var wpis in lista)
            {
                posortowane.Rows.Add(wpis.id, wpis.name, wpis.surname, wpis.city, wpis.phoneNumber);
            }

            ViewState["DaneWidoczne"] = posortowane;
            GridTabela.DataKeyNames = new string[] { "id" };
            GridTabela.DataSource = posortowane;
            GridTabela.DataBind();
        }

        private string GetWartosc(object to, string pole)
        {
            if (pole == "name") return ((dynamic)to).name;
            if (pole == "surname") return ((dynamic)to).surname;
            if (pole == "city") return ((dynamic)to).city;
            if (pole == "phoneNumber") return ((dynamic)to).phoneNumber;
            return "";
        }

        protected void btnDodaj_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtSurname.Text) ||
                string.IsNullOrWhiteSpace(txtCity.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                lblKomunikat.Text = "Wszystkie pola muszą być wypełnione!";
                lblKomunikat.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (!txtPhone.Text.All(char.IsDigit) || txtPhone.Text.Length > 9)
            {
                lblKomunikat.Text = "Numer telefonu musi zawierać tylko cyfry i mieć maksymalnie 9 znaków.";
                lblKomunikat.ForeColor = System.Drawing.Color.Red;
                return;
            }

            XmlDocument dokumentXml = new XmlDocument();
            dokumentXml.Load(lokalizacjaXML);

            int noweId = GetNastepneId();

            XmlElement osoba = dokumentXml.CreateElement("subscriber");
            osoba.SetAttribute("id", noweId.ToString());

            XmlElement imie = dokumentXml.CreateElement("name");
            imie.InnerText = txtName.Text;
            osoba.AppendChild(imie);

            XmlElement nazwisko = dokumentXml.CreateElement("surname");
            nazwisko.InnerText = txtSurname.Text;
            osoba.AppendChild(nazwisko);

            XmlElement miasto = dokumentXml.CreateElement("city");
            miasto.InnerText = txtCity.Text;
            osoba.AppendChild(miasto);

            XmlElement telefon = dokumentXml.CreateElement("phoneNumber");
            telefon.InnerText = txtPhone.Text;
            osoba.AppendChild(telefon);

            dokumentXml.DocumentElement.AppendChild(osoba);
            dokumentXml.Save(lokalizacjaXML);

            ZaladujTabele();
            lblKomunikat.Text = "";
        }

        protected void BtnSzukaj_Click(object sender, EventArgs e)
        {
            string pole = DropSzukajPo.SelectedValue;
            string szukana = txtWyszukaj.Text.Trim().ToLower();

            XmlDocument dokumentXml = new XmlDocument();
            dokumentXml.Load(lokalizacjaXML);
            var rekord = dokumentXml.SelectNodes("//subscriber");

            DataTable wynik = new DataTable();
            wynik.Columns.Add("id");
            wynik.Columns.Add("name");
            wynik.Columns.Add("surname");
            wynik.Columns.Add("city");
            wynik.Columns.Add("phoneNumber");

            foreach (XmlNode fragmentDanych in rekord)
            {
                string id = fragmentDanych.Attributes["id"]?.Value ?? "";
                string name = fragmentDanych["name"]?.InnerText ?? "";
                string surname = fragmentDanych["surname"]?.InnerText ?? "";
                string city = fragmentDanych["city"]?.InnerText ?? "";
                string phone = fragmentDanych["phoneNumber"]?.InnerText ?? "";

                string wartosc = "";
                if (pole == "name") wartosc = name;
                else if (pole == "surname") wartosc = surname;
                else if (pole == "city") wartosc = city;
                else if (pole == "phoneNumber") wartosc = phone;

                if (wartosc.ToLower().Contains(szukana))
                {
                    wynik.Rows.Add(id, name, surname, city, phone);
                }
            }

            ViewState["DaneWidoczne"] = wynik;
            GridTabela.DataKeyNames = new string[] { "id" };
            GridTabela.DataSource = wynik;
            GridTabela.DataBind();
        }

        protected void BtnWyczysc_Click(object sender, EventArgs e)
        {
            txtWyszukaj.Text = "";
            BtnSzukaj_Click(sender, e); // Dodano po wysłaniu listy
            ZaladujTabele();
        }

        private void EksportujDoCsv(DataTable tabela, string fileName)
        {
            if (tabela == null || tabela.Columns.Count == 0)
            {
                Response.Clear();
                Response.ContentType = "text/plain";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.Write("Brak danych do eksportu.");
                Response.End();
                return;
            }

            StringBuilder budowniczy = new StringBuilder();

            budowniczy.AppendLine(string.Join(";", tabela.Columns.Cast<DataColumn>().Select(kolumna => kolumna.ColumnName)));

            foreach (DataRow rzad in tabela.Rows)
            {
                budowniczy.AppendLine(string.Join(";", rzad.ItemArray.Select(pole => pole.ToString())));
            }

            Response.Clear();
            Response.ContentType = "text/csv";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.Write(budowniczy.ToString());
            Response.End();
        }


        private void EksportujDoPdf(DataTable tabela, string plik)
        {
            if (tabela == null || tabela.Columns.Count == 0)
            {
                Response.Clear();
                Response.ContentType = "text/plain";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + plik);
                Response.Write("Brak danych do eksportu.");
                Response.End();
                return;
            }

            Document dokument = new Document();
            MemoryStream strumyk = new MemoryStream();
            PdfWriter.GetInstance(dokument, strumyk);

            dokument.Open();

            PdfPTable tabelaPdf = new PdfPTable(tabela.Columns.Count);
            tabelaPdf.WidthPercentage = 100;

            foreach (DataColumn kolumna in tabela.Columns)
            {
                tabelaPdf.AddCell(new Phrase(kolumna.ColumnName));
            }

            foreach (DataRow rzad in tabela.Rows)
            {
                foreach (var komorka in rzad.ItemArray)
                {
                    tabelaPdf.AddCell(new Phrase(komorka.ToString()));
                }
            }

            dokument.Add(tabelaPdf);
            dokument.Close();

            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + plik);
            Response.BinaryWrite(strumyk.ToArray());
            Response.End();
        }


        protected void btnEksportuj_Click(object sender, EventArgs e)
        {
            string format = DropFormat.SelectedValue;
            string nazwaPliku = txtNazwaEkportowanegoPliku.Text.Trim();

            if (string.IsNullOrWhiteSpace(nazwaPliku))
            {
                nazwaPliku = "dane";
            }

            DataTable dane = ViewState["DaneWidoczne"] as DataTable;
            if (dane == null)
            {
                DataSet tempDane = new DataSet();
                tempDane.ReadXml(lokalizacjaXML);

                if (tempDane.Tables.Count > 0)
                {
                    dane = tempDane.Tables[0];

                    if (!dane.Columns.Contains("id"))
                        dane.Columns.Add("id", typeof(string));

                    XmlDocument dokumentXml = new XmlDocument();
                    dokumentXml.Load(lokalizacjaXML);
                    var rekord = dokumentXml.SelectNodes("//subscriber");

                    for (int i = 0; i < rekord.Count && i < dane.Rows.Count; i++)
                    {
                        var atrybut = rekord[i].Attributes["id"];
                        if (atrybut != null)
                            dane.Rows[i]["id"] = atrybut.Value;
                    }
                }
                else
                {
                    dane = new DataTable();
                    dane.Columns.Add("id");
                    dane.Columns.Add("name");
                    dane.Columns.Add("surname");
                    dane.Columns.Add("city");
                    dane.Columns.Add("phoneNumber");
                }
            }

            if (format == "csv")
            {
                EksportujDoCsv(dane, nazwaPliku + ".csv");
            }
            else if (format == "pdf")
            {
                EksportujDoPdf(dane, nazwaPliku + ".pdf");
            }
        }

        private int GetNastepneId()
        {
            int obecneId = 0;

            if (File.Exists(lokalizacjaBIN))
            {
                using (BinaryReader czytnik = new BinaryReader(File.Open(lokalizacjaBIN, FileMode.Open)))
                {
                    obecneId = czytnik.ReadInt32();
                }
            }

            obecneId++;

            using (BinaryWriter zapisywacz = new BinaryWriter(File.Open(lokalizacjaBIN, FileMode.Create)))
            {
                zapisywacz.Write(obecneId);
            }

            return obecneId;
        }


    }
}
