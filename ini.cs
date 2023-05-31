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
using System.Diagnostics;
using System.Runtime.InteropServices;

public class MainForm : Form
{
    private int StatusCheckBox;
    public MainForm()
    {
        InitializeComponents();
        Icon icon = new Icon("Rnv_Ico.ico");
        this.Icon = icon;
        this.BackColor = Color.FromArgb(54,56,114);
        this.Size = new Size(950, 550);
        this.MinimumSize = new Size(950, 550);
        this.MaximumSize = new Size(950, 550); 
        this.MaximizeBox = false;
    }

    private void InitializeComponents()
    {
        try
        {
            int rest = 0;
            int checkBoxWidth = 208;
            int checkBoxHeight = 20;
            int initialX = 30;
            int initialY = 30;
            int spacing = 10;

            string DiretorioDeExecução = Directory.GetCurrentDirectory();
            string diretorioPai = Path.Combine(DiretorioDeExecução, "..");
            string pastaProcurada = Path.Combine(diretorioPai, "dados");
            string[] subpastas = Directory.GetDirectories(pastaProcurada);

            Panel panel01 = new Panel();
            panel01.Size = new Size(900,100);
            panel01.Location = new Point(10,10);
            panel01.BackColor = Color.Transparent;
            Controls.Add(panel01);

            Text = "Configuração de Novos Bancos de Dados e ODBC";
            Icon = SystemIcons.Information;
            int labelMargin = 5;
            int labelHeight = SystemInformation.CaptionHeight;
            int labelX = Icon.Width + labelMargin;

            Panel rodapéPanel = new Panel();
            rodapéPanel.Location = new Point(0,490);
            rodapéPanel.Size = new Size(950,20);
            rodapéPanel.BackColor = Color.White;
            Controls.Add(rodapéPanel);

            Panel RadioBancoPanel = new Panel();
            RadioBancoPanel.Location = new Point(0,100);
            RadioBancoPanel.Size = new Size(930,340);
            RadioBancoPanel.BackColor = Color.Transparent;
            RadioBancoPanel.AutoScroll = true;
            Controls.Add(RadioBancoPanel);


            TextBox searchTextBox = new TextBox();
            searchTextBox.Location = new Point(30, 30);
            searchTextBox.Size = new Size(930, 50);
            searchTextBox.ForeColor = Color.Black;
            searchTextBox.Text = "Filtrar por nome";
            searchTextBox.TextChanged += (sender, e) => SearchTextBox_TextChanged(sender, e, RadioBancoPanel); 
            panel01.Controls.Add(searchTextBox);

            Label versionLabel = new Label();
            string versaoDP = PegaVersaoDP();
            versionLabel.Text = "Versão do Dispositivo: " + versaoDP;
            versionLabel.BackColor = Color.White;
            versionLabel.Location = new Point(30, 65);
            versionLabel.AutoSize = true;
            panel01.Controls.Add(versionLabel);

            Label Desc1Label = new Label();
            Desc1Label.Text = "Desatualizado";
            Desc1Label.BackColor = Color.Red;
            Desc1Label.ForeColor = Color.White;
            Desc1Label.Location = new Point(750, 65);
            Desc1Label.Size = new Size(85, 20);
            panel01.Controls.Add(Desc1Label);

            Label Desc2Label = new Label();
            Desc2Label.Text = "Atualizado";
            Desc2Label.BackColor = Color.Green;
            Desc2Label.ForeColor = Color.White;
            Desc2Label.Location = new Point(650, 65);
            Desc2Label.Size = new Size(85,20);
            panel01.Controls.Add(Desc2Label);

            CheckBox atualizador = new CheckBox();
            atualizador.Width = 100;
            atualizador.Height = 20;
            atualizador.AutoSize = true;
            atualizador.Location = new Point(345, 65);
            atualizador.Text = "Atualizador de Banco SQL";
            atualizador.BackColor = Color.White;
            atualizador.CheckedChanged += Atualizador_CheckedChanged;
            panel01.Controls.Add(atualizador);

            for (int i = 0; i < subpastas.Length; i++)
            {
                if (rest == 4)
                {
                    rest = 0;
                    initialY += 65;
                    initialX = 30;
                    spacing = 10;

                }
                string folderPath = subpastas[i];
                string folderName = Path.GetFileName(folderPath);
                RadioButton checkBox = new RadioButton();
                checkBox.BackColor = Color.White;
                checkBox.Text = folderName;
                checkBox.Location = new Point(initialX + (checkBoxWidth + spacing) * rest, initialY);
                checkBox.Size = new Size(checkBoxWidth, checkBoxHeight);
                checkBox.CheckedChanged += CheckBox_CheckedChanged;

                Label CheckBoxStatuslabel = new Label();
                string VersaoBanco = PegaVersao(checkBox);
                CheckBoxStatuslabel.Text = "Versão do Banco: "+VersaoBanco;
                if(VersaoBanco != versaoDP){CheckBoxStatuslabel.BackColor = Color.Red;}
                else { CheckBoxStatuslabel.BackColor = Color.Green; }
                if (VersaoBanco == "Pasta Vazia!") { CheckBoxStatuslabel.BackColor = Color.Black; }
                CheckBoxStatuslabel.ForeColor = Color.White;
                CheckBoxStatuslabel.Location = new Point(checkBox.Location.X, checkBox.Location.Y + checkBox.Height);
                CheckBoxStatuslabel.Size = new Size(checkBoxWidth, checkBoxHeight);
                RadioBancoPanel.Controls.Add(CheckBoxStatuslabel);

                RadioBancoPanel.Controls.Add(checkBox);
                rest++;
            }
        }
        catch
        {
            MessageBox.Show("Pasta Dados não encontrada!");
            Environment.Exit(0);
        }
    }

    private string PegaVersaoDP() 
    {
        string DiretorioDeExecuçãoDados = Directory.GetCurrentDirectory();
        string diretorioPaiDados = Path.Combine(DiretorioDeExecuçãoDados, "..");
        string diretorioAtualizador = Path.Combine(diretorioPaiDados, "Acesso.exe");
        string caminhoDoArquivo = diretorioAtualizador;
        string resultVersao = "";
        if (System.IO.File.Exists(caminhoDoArquivo))
        {
            FileVersionInfo informacoesVersao = FileVersionInfo.GetVersionInfo(caminhoDoArquivo);
            string versao = informacoesVersao.FileVersion;
            int index = versao.IndexOf('.', versao.IndexOf('.') + 1);
            resultVersao = versao.Substring(0, index);
        }
        else
        {
            MessageBox.Show("Acesso.exe não encontrado!");
            Environment.Exit(0);
        }
        return resultVersao;

    }

    private string PegaVersao(object sender)
    {
        RadioButton checkBox = (RadioButton)sender;
        string DiretorioDeExecuçãoDados = Directory.GetCurrentDirectory();
        string diretorioPaiDados = Path.Combine(DiretorioDeExecuçãoDados, "..");
        string pastaDados = Path.Combine(diretorioPaiDados, "dados");
        string pastaComBanco = Path.Combine(pastaDados, checkBox.Text);
        string connectionString = "User=SYSDBA;Password=masterkey;Database="+pastaComBanco+"\\DADOSEMP.fdb;DataSource=localhost;Port=3050;Dialect=3;Charset=NONE;";
        string query1 = "select VERSAOPRINCIPAL from modulos_versao WHERE modulo = 'GERENCIAL'";
        string query2 = "select VERSAOMENOR from modulos_versao WHERE modulo = 'GERENCIAL'";
        string VersaoResult1 = "";
        string VersaoResult2 = "";
        try 
        {
            using (FbConnection connection = new FbConnection(connectionString))
            {
                connection.Open();
                using (FbCommand command = new FbCommand(query1, connection))
                {
                    using (FbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            VersaoResult1 = reader.GetString(0);
                        }
                    }
                }
                connection.Close();
            }
            using (FbConnection connection = new FbConnection(connectionString))
            {
                connection.Open();
                using (FbCommand command = new FbCommand(query2, connection))
                {
                    using (FbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            VersaoResult2 = reader.GetString(0);
                        }
                    }
                }
                connection.Close();
            }
            string VersaoTotal = VersaoResult1 + "." + VersaoResult2;
            return VersaoTotal;
        }catch 
        {
            string ErroMSG = "Pasta Vazia!";
            return ErroMSG;
        }
        
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

    private void Atualizador_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox checkBox = (CheckBox)sender;
        if (checkBox.Checked){StatusCheckBox = 1;}
        else{StatusCheckBox = 0;}
    }

    private void CheckAtualizador()
    {
        try 
        {
            string DiretorioDeExecuçãoDados = Directory.GetCurrentDirectory();
            string diretorioPaiDados = Path.Combine(DiretorioDeExecuçãoDados, "..");
            string diretorioAtualizador = Path.Combine(diretorioPaiDados, "Atualizador.exe");
            string caminhoExe = diretorioAtualizador;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = caminhoExe;
            using (Process processo = new Process())
            {
                processo.StartInfo = startInfo;
                processo.Start();
                processo.WaitForExit();
            }
            MessageBox.Show("Banco Configurado com Sucesso!");
        }catch
        {
            MessageBox.Show("Erro! Atualizador.exe não encontrado!\nNão foi possível iniciar o atualizador, mas o banco foi configurado com Sucesso!.");
        }
    }

    private void Criarini(object sender, EventArgs e)
    {
            RadioButton checkBox = (RadioButton)sender;
            DialogResult resultado = MessageBox.Show("O Banco é unificado?", "Caixa de Diálogo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            int uniBanco = 0;
            if (resultado == DialogResult.Yes){uniBanco = 1;}
            else if (resultado == DialogResult.No){uniBanco = 0;}
            string DiretorioDeExecução = Directory.GetCurrentDirectory();
            string diretorioPai = Path.Combine(DiretorioDeExecução, "..");
            string pastaProcurada = Path.Combine(diretorioPai, "dados");
            string[] diretorio = Directory.GetDirectories(pastaProcurada);
            string pastaDestino = @"\renovar";
            string NomeDoArquivo = "renovar.ini";
            string caminhoCompleto = Path.Combine(pastaDestino, NomeDoArquivo);
            List<string> cnpjResult = PegaCNPJ(sender, e);
            List<string> DadosUni = new List<string>();
            
            for (int i = 0; i < 10; i++)
            {
                if (i != 0)
                {
                    if (uniBanco == 0)
                    {
                        DadosUni.Add("0" + (i + 1));
                    }
                    else
                    {
                        DadosUni.Add("");
                    }
                }
                else
                {
                    DadosUni.Add("");
                }
            }

            for(int i = 0;i< 10; i++)
            {
                if (i >= cnpjResult.Count)
                {
                    cnpjResult.Add("sem CNPJ");
                }
                else if (string.IsNullOrEmpty(cnpjResult[i]))
                {
                    cnpjResult[i] = "sem CNPJ";
                }
                else if (cnpjResult[i] == "00000000000000") 
                {
                    for(int j = 0; j < cnpjResult.Count; j++) 
                    {
                        if(cnpjResult[j] != "00000000000000"){cnpjResult[i] = cnpjResult[j];}
                    }

                }
            }
        string DadosDoArquivo = @"
[SISTEMA]
DADOS01=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados.fdb
DADOS02=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados"+DadosUni[1]+".fdb"+@"
DADOS03=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados"+DadosUni[2]+".fdb"+@"
DADOS04=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados"+DadosUni[3]+".fdb"+@"
DADOS05=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados"+DadosUni[4]+".fdb"+@"
DADOS06=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados"+DadosUni[5]+".fdb"+@"
DADOS07=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados"+DadosUni[5]+".fdb"+@"
DADOS08=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados"+DadosUni[5]+".fdb"+@"
DADOS09=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados"+DadosUni[5]+".fdb"+@"
DADOSEMP=C:\RENOVAR\DADOS\" + checkBox.Text + @"\DadosEmp.fdb
DADOSREDE01=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados.fdb
DADOSREDE02=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados02.fdb
DADOSREDE03=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados03.fdb
DADOSREDE04=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados04.fdb
DADOSREDE05=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados05.fdb
DADOSREDE06=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados06.fdb
DADOSREDE07=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados07.fdb
DADOSREDE08=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados08.fdb
DADOSREDE09=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados09.fdb
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
"CNPJ09=" + cnpjResult[8]+@"
SERVIDOR=LOCALHOST

[REDE]
DADOSREDE01=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Dados.fdb
DADOSREDEEMP=C:\RENOVAR\DADOS\" + checkBox.Text + @"\DadosEmp.fdb
DADOSREDELOG=C:\RENOVAR\DADOS\" + checkBox.Text + @"\Log.fdb
SERVIDORREDE=LOCALHOST

[SQL]
DADOS_SQL01=Dados01
DADOS_SQLEMP=DadosEmp
DADOS_SQLLOG=DadosLog" + "\n" +

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
UNIFICADA=" + uniBanco+"\n"+@"

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
EMPRESA=1";
        using (StreamWriter writer = new StreamWriter(caminhoCompleto)){writer.WriteLine(DadosDoArquivo);}
    }
    private IEnumerable<string> BuscarArquivosDados(object sender, EventArgs e)
    {
        IEnumerable<string> returne = Enumerable.Empty<string>();
        try
        {
        RadioButton checkBox = (RadioButton)sender;
        string DiretorioDeExecuçãoDados = Directory.GetCurrentDirectory();
        string diretorioPaiDados = Path.Combine(DiretorioDeExecuçãoDados, "..");
        string pastaDados = Path.Combine(diretorioPaiDados, "dados");
        string pastaComBanco = Path.Combine(pastaDados, checkBox.Text);
        string[] arquivos = Directory.GetFiles(pastaComBanco, "*DADOS*").Where(arquivo => !Path.GetFileName(arquivo).Contains("DADOSEMP")).ToArray();
        return arquivos;
        }catch{return returne;}
    }

    private void CheckBox_CheckedChanged(object sender, EventArgs e)
    {
        RadioButton checkBox = (RadioButton)sender;
        if (checkBox.Checked)
        {
            int i = 0;
            string pastaComBanco;
            IEnumerable<string> arquivosFiltrados = BuscarArquivosDados(sender, e);
            if(arquivosFiltrados.Any()){}
            else
            {
                MessageBox.Show("Nenhum Arquivo DADOS.FDB encontrado!");
                return;
            }
            Criarini(sender, e);
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
                    if (odbcKey != null) { odbcKey.Close();}
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
                if (StatusCheckBox == 1){CheckAtualizador();}
                else{MessageBox.Show("Banco Configurado com Sucesso!");}
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um erro: " + ex.Message);
            }
        }
    }

    private void SearchTextBox_TextChanged(object sender, EventArgs e, Panel RadioBancoPanel)
    {
        TextBox textBox = (TextBox)sender;
        Label CheckBoxStatuslabel = new Label();
        string versaoDP = PegaVersaoDP();
        string searchText = textBox.Text;
        string VersaoBanco;
        int rest = 0;

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            string termoDeBusca = searchText;
            Control.ControlCollection controles = RadioBancoPanel.Controls;
            while (controles.Count > 0)
            {
                Control controle = controles[0];
                RadioBancoPanel.Controls.Remove(controle);
                controle.Dispose();
            }

            int checkBoxWidth = 200;
            int checkBoxHeight = 20;
            int initialX = 30;
            int initialY = 30;
            int spacing = 10;


                string DiretorioDeExecução = Directory.GetCurrentDirectory();
                string diretorioPai = Path.Combine(DiretorioDeExecução, "..");
                string pastaProcurada = Path.Combine(diretorioPai, "dados");
                string[] diretorio = Directory.GetDirectories(pastaProcurada);
                IEnumerable<string> pastasFiltradas = diretorio.Where(pasta => pasta.IndexOf(termoDeBusca, StringComparison.OrdinalIgnoreCase) >= 0);
                int i = 0;
                foreach (string pasta in pastasFiltradas)
                {                
                    string folderPath = pasta.ToString();
                    string folderName = Path.GetFileName(folderPath);
                    RadioButton checkBoxExistente = Controls.OfType<RadioButton>().FirstOrDefault(control => string.Equals(termoDeBusca, folderName, StringComparison.OrdinalIgnoreCase)) as RadioButton;
                    if (checkBoxExistente == null)
                    {
                            RadioButton checkBox = new RadioButton();
                            if (rest == 4)
                            {
                                rest=0;
                                initialY += 65;
                                initialX = 30;
                                spacing = 10;

                            }
                            checkBox.BackColor = Color.White;
                            checkBox.Text = folderName;
                            checkBox.Location = new Point(initialX + (checkBoxWidth + spacing) * rest, initialY);
                            checkBox.Size = new Size(checkBoxWidth, checkBoxHeight);
                            checkBox.CheckedChanged += CheckBox_CheckedChanged;
                            RadioBancoPanel.Controls.Add(checkBox);

                            CheckBoxStatuslabel = new Label();
                            VersaoBanco = PegaVersao(checkBox);
                            CheckBoxStatuslabel.Text = "Versão do Banco: " + VersaoBanco;
                            CheckBoxStatuslabel.ForeColor = Color.White;
                            if (VersaoBanco != versaoDP) { CheckBoxStatuslabel.BackColor = Color.Red; }
                            else { CheckBoxStatuslabel.BackColor = Color.Green; }
                            if (VersaoBanco == "Pasta Vazia!") { CheckBoxStatuslabel.BackColor = Color.Black; }
                            CheckBoxStatuslabel.Location = new Point(checkBox.Location.X, checkBox.Location.Y + checkBox.Height);
                            CheckBoxStatuslabel.Size = new Size(checkBoxWidth, checkBoxHeight);
                            RadioBancoPanel.Controls.Add(CheckBoxStatuslabel);
                }
                    rest++;
                    i++;
                }
        }
        else
        {
            Control.ControlCollection controles = RadioBancoPanel.Controls;
            while (controles.Count > 0)
            {
                Control controle = controles[0];
                RadioBancoPanel.Controls.Remove(controle);
                controle.Dispose(); 
            }

            int checkBoxWidth = 200;
            int checkBoxHeight = 20; 
            int initialX = 30;
            int initialY = 30;
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
                if(rest == 4) 
                {
                    rest = 0;
                    initialY += 65;
                    checkBoxWidth = 200;
                    checkBoxHeight = 20;
                    initialX = 30;
                    spacing = 10;

                }
                checkBox.BackColor = Color.White;
                checkBox.Text = folderName;
                checkBox.Location = new Point(initialX + (checkBoxWidth + spacing) * rest, initialY);
                checkBox.Size = new Size(checkBoxWidth, checkBoxHeight);
                checkBox.CheckedChanged += CheckBox_CheckedChanged;
                RadioBancoPanel.Controls.Add(checkBox);

                CheckBoxStatuslabel = new Label();
                VersaoBanco = PegaVersao(checkBox);
                CheckBoxStatuslabel.Text = "Versão do Banco: " + VersaoBanco;
                CheckBoxStatuslabel.ForeColor = Color.White;
                if (VersaoBanco != versaoDP) { CheckBoxStatuslabel.BackColor = Color.Red; }
                else { CheckBoxStatuslabel.BackColor = Color.Green; }
                if (VersaoBanco == "Pasta Vazia!") { CheckBoxStatuslabel.BackColor = Color.Black; }
                CheckBoxStatuslabel.Location = new Point(checkBox.Location.X, checkBox.Location.Y + checkBox.Height);
                CheckBoxStatuslabel.Size = new Size(checkBoxWidth, checkBoxHeight);
                RadioBancoPanel.Controls.Add(CheckBoxStatuslabel);
                rest++;
            }
        }


    }

    public static void Main()
    {
        try { Application.Run(new MainForm()); }
        catch(Exception ex) { MessageBox.Show("Erro ao iniciar o aplicativo: " + ex.Message); }
    }
}


