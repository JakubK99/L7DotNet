<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Zadanie_1.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Strona główna</title>
    <link rel="stylesheet" href="css/Style.css"/>
</head>
<body>
    <form id="form1" runat="server">
        <section id="dodawanie">
            <div>
                <asp:Label ID="LblDodawanie" runat="server" Text="Dodawanie" ForeColor="White" Font-Names="Courier New" Font-Bold="true"></asp:Label>
            </div>
            <asp:TextBox ID="txtName" runat="server" Placeholder="Imię"></asp:TextBox>
            <asp:TextBox ID="txtSurname" runat="server" Placeholder="Nazwisko"></asp:TextBox>
            <asp:TextBox ID="txtCity" runat="server" Placeholder="Miasto"></asp:TextBox>
            <asp:TextBox ID="txtPhone" runat="server" Placeholder="Telefon"></asp:TextBox>
            <asp:Button ID="btnDodaj" runat="server" Text="Dodaj" onClick="btnDodaj_Click"/>
            <br />
            <asp:Label ID="lblKomunikat" runat="server" Font-Names="Courier New"></asp:Label>
        </section>
        <section id="szukanie">
            <div>
                <asp:Label ID="lblSzukanie" runat="server" Text="Szukanie" ForeColor="White" Font-Names="Courier New" Font-Bold="true"></asp:Label>
            </div>
            <asp:DropDownList ID="DropSzukajPo" runat="server">
            <asp:ListItem Text="Imię" Value="name" />
            <asp:ListItem Text="Nazwisko" Value="surname" />
            <asp:ListItem Text="Miasto" Value="city" />
            <asp:ListItem Text="Numer telefonu" Value="phoneNumber" />
            </asp:DropDownList>

            <asp:TextBox ID="txtWyszukaj" runat="server" Placeholder="Wpisz szukaną frazę..." />

            <asp:Button ID="BtnSzukaj" runat="server" Text="Szukaj" OnClick="BtnSzukaj_Click" />
            <asp:Button ID="BtnWyczysc" runat="server" Text="Wyczyść" OnClick="BtnWyczysc_Click" />
        </section>
        <section id="sortowanie">
            <div>
                <asp:Label ID="LblSortowanie" runat="server" Text="Sortowanie" ForeColor="White" Font-Names="Courier New" Font-Bold="true"></asp:Label>
            </div>
            <asp:DropDownList ID="DropSortujPole" runat="server">
                <asp:ListItem Text="Imię" Value="name" />
                <asp:ListItem Text="Nazwisko" Value="surname" />
                <asp:ListItem Text="Miasto" Value="city" />
                <asp:ListItem Text="Numer telefonu" Value="phoneNumber" />
            </asp:DropDownList>

            <asp:DropDownList ID="DropKierunek" runat="server">
                <asp:ListItem Text="Rosnąco" Value="asc" />
                <asp:ListItem Text="Malejąco" Value="desc" />
            </asp:DropDownList>

            <asp:Button ID="BtnSortuj" runat="server" Text="Zatwierdź" OnClick="BtnSortuj_Click" />
        </section>
        <section id="eksport">
            <div>
                <asp:Label ID="lblEksport" runat="server" Text="Eksport danych" ForeColor="White" Font-Names="Courier New" Font-Bold="true"></asp:Label>
            </div>
            <asp:DropDownList ID="DropFormat" runat="server">
                <asp:ListItem Text=".csv" Value="csv" />
                <asp:ListItem Text=".pdf" Value="pdf" />
            </asp:DropDownList>

            <asp:TextBox ID="txtNazwaEkportowanegoPliku" runat="server" Placeholder="Nazwa pliku (bez rozszerzenia)" />

            <asp:Button ID="btnEksportuj" runat="server" Text="Zapisz" OnClick="btnEksportuj_Click" />
        </section>
        <section id="tabela">
            <asp:GridView ID="GridTabela" runat="server" AutoGenerateColumns="false" GridLines="None" BorderStyle="None" BorderWidth="0px" CellPadding="0" CellSpacing="0" OnRowEditing="GridTabela_RowEditing" OnRowUpdating="GridTabela_RowUpdating" OnRowCancelingEdit="GridTabela_RowCancelingEdit" OnRowDeleting="GridTabela_RowDeleting" OnRowDataBound="GridTabela_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="name" HeaderText="Imię" />
                    <asp:BoundField DataField="surname" HeaderText="Nazwisko" />
                    <asp:BoundField DataField="city" HeaderText="Miasto" />
                    <asp:BoundField DataField="phoneNumber" HeaderText="Numer telefonu" />

                    <asp:CommandField ShowEditButton="true" ShowDeleteButton="true" HeaderText="Operacje"/>
                </Columns>
            </asp:GridView>
        </section>
    </form>
</body>
</html>
