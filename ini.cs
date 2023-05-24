using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Data.Odbc;
using Microsoft.Win32;
using FirebirdSql.Data.FirebirdClient;
using System.Linq.Expressions;

public class MainForm : Form
{    
    public MainForm()
    {
        InitializeComponents();
        Icon icon = new Icon("Rnv_Ico.ico");
        this.Icon = icon;
        this.BackColor = Color.FromArgb(54,56,114);
        this.Size = new Size(1000, 800);
        this.SizeChanged += Form1_SizeChanged;
    }

    private void InitializeComponents()
    {
        try
        {
            int checkBoxWidth = 100;
            int checkBoxHeight = 20;
            int initialX = 50;
            int initialY = 150;
            int spacing = 10;
            string DiretorioDeExecução = Directory.GetCurrentDirectory();
            string diretorioPai = Path.Combine(DiretorioDeExecução, "..");
            string pastaProcurada = Path.Combine(diretorioPai, "dados");
            string[] PastasEncontradas = Directory.GetDirectories(pastaProcurada);

            for (int i = 0; i < PastasEncontradas.Length; i++)
            {
                string folderPath = PastasEncontradas[i];
                string folderName = Path.GetFileName(folderPath);

                RadioButton checkBox = new RadioButton();
                checkBox.BackColor = Color.White;
                checkBox.Text = folderName;
                checkBox.Location = new Point(initialX + (checkBoxWidth + spacing) * i, initialY);
                checkBox.Size = new Size(checkBoxWidth, checkBoxHeight);
                checkBox.CheckedChanged += CheckBox_CheckedChanged;
                Controls.Add(checkBox);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show("Ocorreu um erro: " + ex.Message);
        }
        TextBox searchTextBox = new TextBox();
        searchTextBox.Location = new Point(50, 35);
        searchTextBox.Size = new Size(200, 50);
        searchTextBox.ForeColor = Color.Black;
        searchTextBox.Text = "Filtrar por nome";
        searchTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
        searchTextBox.TextChanged += new EventHandler(SearchTextBox_TextChanged);

        Controls.Add(searchTextBox);
    }

    private List<string> PegaCNPJ(object sender, EventArgs e)
    {
        RadioButton checkBox = (RadioButton)sender;
        string DiretorioDeExecuçãoDados = Directory.GetCurrentDirectory();
        string diretorioPaiDados = Path.Combine(DiretorioDeExecuçãoDados, "..");
        string pastaDados = Path.Combine(diretorioPaiDados, "dados");
        string pastaComBanco = Path.Combine(pastaDados, checkBox.Text);
        string connectionString = "User=SYSDBA;Password=masterkey;Database="+pastaComBanco+"\\DADOSEMP.fdb;DataSource=localhost;Port=3050;Dialect=3;Charset=NONE;";
        string query = "SELECT cnpjemp FROM empresa";
        List<string> cnpjResult = new List<string>();
        using (FbConnection connection = new FbConnection(connectionString))
        {
            connection.Open();

            using (FbCommand command = new FbCommand(query, connection))
            {
                using (FbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string cnpj = reader.GetString(0);
                        cnpjResult.Add(cnpj);
                    }
                }
            }

            connection.Close();
        }
        return cnpjResult;
    }

    private void Criarini(object sender, EventArgs e)
    {
            RadioButton checkBox = (RadioButton)sender;
            DialogResult resultado = MessageBox.Show("O Banco é unificado?", "Caixa de Diálogo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            int uniBanco = 0;
            if (resultado == DialogResult.Yes)
            {
                uniBanco = 1;
            }
            else if (resultado == DialogResult.No)
            {
                uniBanco = 0;
            }
            string DiretorioDeExecução = Directory.GetCurrentDirectory();
            string diretorioPai = Path.Combine(DiretorioDeExecução, "..");
            string pastaProcurada = Path.Combine(diretorioPai, "dados");
            string[] diretorio = Directory.GetDirectories(pastaProcurada);

            string pastaDestino = @"\renovar";
            string NomeDoArquivo = "renovar.ini";
            string caminhoCompleto = Path.Combine(pastaDestino, NomeDoArquivo);
            List<string> cnpjResult = PegaCNPJ(sender, e);
            for (int i = 1; i < 10; i++)
            {
                if (i <= cnpjResult.Count && !string.IsNullOrEmpty(cnpjResult[i - 1]))
                {
                    cnpjResult.Insert(i, "sem CNPJ");
                }
            }

        string DadosDoArquivo = @"
DADOS01=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados.fdb
DADOSEMP=C:\RENOVAR\DADOS\" + checkBox.Text + @"\DadosEmp.fdb
DADOSLOG=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Log.fdb
SERVIDOR=LOCALHOST

DADOSREDE01=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados.fdb
DADOSREDEEMP=C:\RENOVAR\DADOS\" + checkBox.Text + @"\DadosEmp.fdb
DADOSREDELOG=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Log.fdb
SERVIDORREDE=LOCALHOST

DADOS_SQL01=Dados01
DADOS_SQLEMP=DadosEmp
DADOS_SQLLOG=DadosLog"+

"\n" + "CNPJ01=" +cnpjResult[0]+ @"
DADOS02 =C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados02.fdb
DADOS03=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados03.fdb
DADOS04=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados04.fdb
DADOS05=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados05.fdb
DADOS06=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados06.fdb
DADOSREDE02=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados02.fdb
DADOSREDE03=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados03.fdb
DADOSREDE04=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados04.fdb
DADOSREDE05=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados05.fdb
DADOSREDE06=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados06.fdb"+
"\n" + "CNPJ02=" + cnpjResult[1] +"\n" +
"CNPJ03=" + cnpjResult[2] + "\n" +
"CNPJ04=" + cnpjResult[3] + "\n" +
"CNPJ05=" + cnpjResult[4] + "\n" +
"CNPJ06=" + cnpjResult[5] + "\n" +
"CNPJ07=" + cnpjResult[6] + "\n" +
"CNPJ08=" + cnpjResult[7] + "\n" +
"CNPJ09=" +cnpjResult[8] +"\n" +

@"[HOST]
HOST01=DESENV01\SQL2008
HOST02=DESENV01\SQL2008
HOST03=DESENV01\SQL2008
HOST04=DESENV01\SQL2008
HOST05=DESENV01\SQL2008
HOST06=DESENV01\SQL2008
HOST07=DESENV01\SQL2008
HOST08=DESENV01\SQL2008
HOST09=DESENV01\SQL2008
HOST10=DESENV01\SQL2008
HOSTEMP=DESENV01\SQL2008
HOSTLOG=DESENV01\SQL2008

[DATABASE]
SGBD=01
UNIFICADA="+ uniBanco+"\n"+@"

[DATABASE VERSION]
VERSION=2008

[Tipo SGDB]
Firebird = 01
SQLServer = 02

[ECF]
IMPRESSORA=2
MFD=S

[Tipo Impressora]
BEMATECH = 1
SWEDA = 2
DARUMA = 3

[Atualizacao]
Repositorio=C:\RENOVAR\
[LAYOUT]
SIZE=6
[TEMA]
NOME=Office2010Blue
[GESTORONLINE]
HOSTNAME=
USERNAME=
PASSWORD=
DATABASE=
PORT=
EMPRESA=1
[SISTEMA]
DADOS01=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados.fdb
DADOS02=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados02.fdb
DADOS03=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados03.fdb
DADOS04=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados04.fdb
DADOS05=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados05.fdb
DADOS06=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados06.fdb
DADOSEMP=C:\RENOVAR\DADOS\" + checkBox.Text + @"\DadosEmp.fdb
DADOSREDE01=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados.fdb
DADOSREDE02=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados02.fdb
DADOSREDE03=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados03.fdb
DADOSREDE04=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados04.fdb
DADOSREDE05=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados05.fdb
DADOSREDE06=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados06.fdb
DADOSREDEEMP=C:\RENOVAR\DADOS\" + checkBox.Text + @"\DadosEmp.fdb
DADOSLOG=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Log.fdb
DADOSREDELOG=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Log.fdb
CNPJ01="+ cnpjResult[0] +"\n" +
"CNPJ02=" + cnpjResult[1] +"\n" +
"CNPJ03=" + cnpjResult[2] + "\n" +
"CNPJ04=" + cnpjResult[3] + "\n" +
"CNPJ05=" + cnpjResult[4] + "\n" +
"CNPJ06=" + cnpjResult[5] + "\n" +
"CNPJ07=" + cnpjResult[6] + "\n" +
"CNPJ08=" + cnpjResult[7] + "\n" +
"CNPJ09=" + cnpjResult[8];
        using (StreamWriter writer = new StreamWriter(caminhoCompleto))
        {
            writer.WriteLine(DadosDoArquivo);
        }
    }

    private IEnumerable<string> BuscarArquivosDados(object sender, EventArgs e)
    {
        RadioButton checkBox = (RadioButton)sender;
        string DiretorioDeExecuçãoDados = Directory.GetCurrentDirectory();
        string diretorioPaiDados = Path.Combine(DiretorioDeExecuçãoDados, "..");
        string pastaDados = Path.Combine(diretorioPaiDados, "dados");
        string pastaComBanco = Path.Combine(pastaDados, checkBox.Text);
        string[] arquivos = Directory.GetFiles(pastaComBanco, "*DADOS*")
            .Where(arquivo => !Path.GetFileName(arquivo).Contains("DADOSEMP")).ToArray();

        return arquivos;
    }

    private void CheckBox_CheckedChanged(object sender, EventArgs e)
    {
        RadioButton checkBox = (RadioButton)sender;

        if (checkBox.Checked)
        {
           Criarini(sender, e);

            int i = 0;
            string pastaComBanco;
            IEnumerable<string> arquivosFiltrados = BuscarArquivosDados(sender, e);
            try
            {
                foreach (string arquivos in arquivosFiltrados)
                {
                    i++;
                    string DiretorioDeExecuçãoDados = Directory.GetCurrentDirectory();
                    string diretorioPaiDados = Path.Combine(DiretorioDeExecuçãoDados, "..");
                    string pastaDados = Path.Combine(diretorioPaiDados, "dados");
                    if (i > 1) { pastaComBanco = Path.Combine(pastaDados, checkBox.Text, "dados0" + i + ".fdb"); }
                    else { pastaComBanco = Path.Combine(pastaDados, checkBox.Text, "dados.fdb"); }
                    string dsnName = "RENOVARFB0" + i;
                    string driverName = "Firebird/InterBase(r) driver";
                    string databasePath = pastaComBanco;
                    string username = "SYSDBA";
                    string password = "masterkey";
                    string Descrição = "Dados0" + i;
                    string Client = "C:\\Program Files (x86)\\Firebird\\Firebird_3_0\\fbclient.dll";
                    string dsnConnectionString = $"DRIVER={{{driverName}}};DBNAME={databasePath};UID={username};PWD={password};";
                    if (Registry.CurrentUser.OpenSubKey("Software\\ODBC\\ODBC.INI\\ODBC Data Sources") == null)
                    {
                        Registry.CurrentUser.CreateSubKey("Software\\ODBC\\ODBC.INI\\ODBC Data Sources");
                    }
                    RegistryKey odbcKey = Registry.CurrentUser.OpenSubKey("Software\\ODBC\\ODBC.INI\\ODBC Data Sources", true);
                    odbcKey.SetValue(dsnName, driverName);
                    if (odbcKey != null) { odbcKey.Close(); }
                    RegistryKey dsnKey = Registry.CurrentUser.CreateSubKey("Software\\ODBC\\ODBC.INI\\" + dsnName);
                    if (dsnKey != null)
                    {
                        dsnKey.SetValue("Driver", driverName);
                        dsnKey.SetValue("Dbname", databasePath);
                        dsnKey.SetValue("User", username);
                        dsnKey.SetValue("Password", password);
                        dsnKey.SetValue("Client", Client);
                        dsnKey.SetValue("Description", Descrição);
                        dsnKey.Close();
                    }
                }
                MessageBox.Show("Banco Configurado com Sucesso!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um erro: " + ex.Message);
            }
        }
    }

    private void Form1_SizeChanged(object sender, EventArgs e)
    {
        //ajuste dos RadioButton quando o tamanho da tela for alterado
        AjustarRadioButtons();
    }

    private void AjustarRadioButtons()
    {
        int checkBoxWidth = 100; // Largura desejada para cada RadioButton
        int checkBoxHeight = 20; // Altura desejada para cada RadioButton
        int initialX = 50; // Posição inicial horizontal (X)
        int initialY = 150; // Posição inicial vertical (Y)
        int spacing = 10; // Espaçamento entre os RadioButton

        // Calcula o número de colunas com base no tamanho atual da tela e a largura dos RadioButton
        int numColumns = Math.Max((ClientSize.Width - initialX) / (checkBoxWidth + spacing), 1);

        // Calcula a largura real de cada coluna considerando o espaçamento entre RadioButton
        int columnWidth = (ClientSize.Width - initialX) / numColumns;

        // Obtém a altura disponível com base no tamanho atual da tela e na posição vertical inicial
        int availableHeight = ClientSize.Height - initialY;

        // Calcula o número de linhas com base na altura disponível e na altura dos RadioButton
        int numRows = Math.Max(availableHeight / (checkBoxHeight + spacing), 1);

        // Atualiza as posições e tamanhos dos RadioButton
        for (int i = 0; i < Controls.Count; i++)
        {
            Control control = Controls[i];
            if (control is RadioButton radioButton)
            {
                int column = i % numColumns;
                int row = i / numColumns;

                int x = initialX + column * (columnWidth + spacing);
                int y = initialY + row * (checkBoxHeight + spacing);

                radioButton.Location = new Point(x, y);
                radioButton.Size = new Size(columnWidth, checkBoxHeight);
            }
        }
    }

    private void SearchTextBox_TextChanged(object sender, EventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        string searchText = textBox.Text;

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            string termoDeBusca = searchText; 
           

            foreach (var control in Controls.OfType<RadioButton>().ToList())
            {
               Controls.Remove(control);
            }
                int checkBoxWidth = 100;
                int checkBoxHeight = 20;
                int initialX = 50;
                int initialY = 150;
                int spacing = 10;
                string DiretorioDeExecução = Directory.GetCurrentDirectory();
                string diretorioPai = Path.Combine(DiretorioDeExecução, "..");
                string pastaProcurada = Path.Combine(diretorioPai, "dados");
                string[] diretorio = Directory.GetDirectories(pastaProcurada);
                IEnumerable<string> pastasFiltradas = diretorio.Where(pasta => pasta.IndexOf(termoDeBusca, StringComparison.OrdinalIgnoreCase) >= 0);
                int i = -1;
                foreach (string pasta in pastasFiltradas)
                {
                    
                    i++;

                    string folderPath = pasta.ToString();

                    string folderName = Path.GetFileName(folderPath);
                    RadioButton checkBoxExistente = Controls.OfType<RadioButton>()
                        .FirstOrDefault(control => string.Equals(termoDeBusca, folderName, StringComparison.OrdinalIgnoreCase)) as RadioButton;

                if (checkBoxExistente == null)
                    {
                        RadioButton checkBox = new RadioButton();
                        checkBox.BackColor = Color.White;
                        checkBox.Text = folderName;
                        checkBox.Location = new Point(initialX + (checkBoxWidth + spacing) * i, initialY);
                        checkBox.Size = new Size(checkBoxWidth, checkBoxHeight);


                    checkBox.Size = new Size(checkBoxWidth, checkBoxHeight);
                        checkBox.CheckedChanged += CheckBox_CheckedChanged;
                        Controls.Add(checkBox);
                }
                    else
                    {}
                }
        }
        else 
        {
            foreach (var control in Controls.OfType<RadioButton>().ToList())
            {
                Controls.Remove(control);
            }

            int checkBoxWidth = 100;
            int checkBoxHeight = 20; 
            int initialX = 50;
            int initialY = 150;
            int spacing = 10;
            string DiretorioDeExecução = Directory.GetCurrentDirectory();
            string diretorioPai = Path.Combine(DiretorioDeExecução, "..");
            string pastaProcurada = Path.Combine(diretorioPai, "dados");
            string[] subpastas = Directory.GetDirectories(pastaProcurada);

            for (int i = 0; i < subpastas.Length; i++)
            {
                string folderPath = subpastas[i];
                string folderName = Path.GetFileName(folderPath);

                RadioButton checkBox = new RadioButton();
                checkBox.BackColor = Color.White;
                checkBox.Text = folderName;
                checkBox.Location = new Point(initialX + (checkBoxWidth + spacing) * i, initialY);
                checkBox.Size = new Size(checkBoxWidth, checkBoxHeight);
                checkBox.CheckedChanged += CheckBox_CheckedChanged;
                Controls.Add(checkBox);
            }
        }


    }

    public static void Main()
    {
        Application.Run(new MainForm());
    }
}


