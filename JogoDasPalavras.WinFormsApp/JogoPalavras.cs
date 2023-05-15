using JogoDasPalavras.ClassLibrary;
using static JogoDasPalavras.ClassLibrary.Jogo;

namespace JogoDasPalavras.WinFormsApp;

public partial class JogoPalavras : Form
{
    private Jogo jogo = null!;
    private List<RichTextBox> listRich = null!;
    private List<Button> listButton = null!;
    private RichTextBox caixaSelecionada = new RichTextBox();

    public JogoPalavras()
    {
        InitializeComponent();
        CriarListaTextBox();
        CriarListaBotoes();
        CriarEventosBotoes();
        IniciarJogo();
    }


    private void CriarEventosBotoes()
    {
        foreach (var item in grupoTeclas.Controls)
        {
            if (item is Button btn && btn.Name != "buttonEnter" && btn.Name != "backspace")
            {
                btn.Click += IncluirLetraPorTecladoVirtual!;
            }
        }
    }

    private void CriarListaTextBox()
    {
        listRich = new List<RichTextBox>();

        foreach (Control item in grupoQuadros.Controls)
        {
            if (item is RichTextBox richTBox)
            {
                listRich.Add(richTBox);

                richTBox.Click += CaixaSelecionada_click;
                richTBox.KeyDown += AlterarCaixaBackSpace_KeyDown!;
                richTBox.TextChanged += AlterarCaixaPreenchida_TextChanged!;
                richTBox.Click += AlterarCaixa_Click!;

                richTBox.SelectionAlignment = HorizontalAlignment.Center;

            }
        }
    }

    private void AlterarCaixa_Click(object sender, EventArgs e)
    {
        this.caixaSelecionada = (RichTextBox)sender;
    }

    private void AlterarCaixaBackSpace_KeyDown(object sender, KeyEventArgs e)
    {
        RichTextBox txtBox = (RichTextBox)sender;

        if (e.KeyCode == Keys.Back && txtBox.Text == "")
        {
            SendKeys.Send("+{TAB}");

            e.Handled = e.SuppressKeyPress = true;
        }
    }

    private void AlterarCaixaPreenchida_TextChanged(object sender, EventArgs e)
    {
        RichTextBox txtBox = (RichTextBox)sender;

        this.caixaSelecionada = txtBox;

        if (txtBox.Text != string.Empty)
        {
            SendKeys.Send("{TAB}");
        }
    }

    private void CriarListaBotoes()
    {
        listButton = new List<Button>();

        foreach (var item in grupoTeclas.Controls)
        {
            if (item is Button btn)
            {
                listButton.Add(btn);
            }
        }
    }

    private void ColorirCelulas(List<Resultado> resultado)
    {
        int index = 0;

        var listaJogadaAtual = BuscarRowJogada();

        foreach (Resultado item in resultado)
        {
            RichTextBox letra = listaJogadaAtual.First(i => i.Tag.Equals(index.ToString()));

            Button btn = listButton.Find(i => i.Text.Equals(letra.Text.ToUpper()))!;

            if (btn != null && item == Resultado.ERRADO)
            {
                btn.BackColor = Color.DarkGray;
            }

            switch (item)
            {
                case Resultado.ERRADO: letra.BackColor = Color.LightGray; break;
                case Resultado.FORA_ORDEM: letra.BackColor = Color.Yellow; break;
                case Resultado.CERTO: letra.BackColor = Color.LightGreen; break;
            }
            index++;
        }
    }

    private IOrderedEnumerable<RichTextBox> BuscarRowJogada()
    {
        List<RichTextBox> listaAtiva = new();

        foreach (RichTextBox txtBox in listRich)
        {
            if (grupoQuadros.GetRow(txtBox) == jogo.NumeroJogada)
            {
                listaAtiva.Add(txtBox);
            }
        }
        return listaAtiva.OrderBy(i => i.Tag);
    }

    private string ConstruirStringPalpite()
    {
        string palpite = "";

        while (palpite.Length != 5)
        {
            foreach (RichTextBox item in BuscarRowJogada())
            {
                if (item.Enabled == true && item.Tag.ToString() == palpite.Length.ToString())
                {
                    palpite += item.Text;
                }
            }
;
        }

        return palpite;
    }

    private bool PalavraTemCincoLetras()
    {
        string palpite = "";

        foreach (RichTextBox item in BuscarRowJogada())
        {
            palpite += item.Text;
        }
        return palpite.Trim().Length == 5;
    }

    private void buttonEnter_Click(object sender, EventArgs e)
    {
        VerificarSeAcertou();
    }

    private void AtivarColunas()
    {
        foreach (Control item in grupoQuadros.Controls)
        {
            if (item is RichTextBox richTBox)
            {
                int linha = grupoQuadros.GetRow(richTBox);

                if (linha == jogo.NumeroJogada)
                {
                    richTBox.Enabled = true;
                    caixaSelecionada = BuscarRowJogada().ElementAt(0);
                    caixaSelecionada.Focus();
                }
                else if (linha < jogo.NumeroJogada)
                {
                    richTBox.ReadOnly = true;
                    richTBox.TabStop = false;
                }
                else richTBox.Enabled = false;
            }
        }
    }

    private void LimparColunas()
    {
        foreach (RichTextBox richTBox in listRich)
        {
            richTBox.ReadOnly = false;
            richTBox.TabStop = true;
            richTBox.Text = string.Empty;
            richTBox.BackColor = Color.White;
        }

        listButton.ForEach(b => b.BackColor = Color.White);
    }

    private void CaixaSelecionada_click(object? sender, EventArgs e)
    {
        RichTextBox txtBox = (RichTextBox)sender!;
        this.caixaSelecionada = txtBox!;
    }

    private void IncluirLetraPorTecladoVirtual(object sender, EventArgs e)
    {
        Button btn = (Button)sender;

        caixaSelecionada.Text = btn.Text;

        int tagProximaCaixa = int.Parse(caixaSelecionada.Tag.ToString()!) + 1;

        if (tagProximaCaixa > 4)
            return;

        caixaSelecionada.Focus();
        caixaSelecionada = BuscarRowJogada().ElementAt(tagProximaCaixa);
    }

    private void IniciarJogo()
    {
        jogo = new Jogo();

        LimparColunas();
        AtivarColunas();

        labelPalavra.Text = jogo.PalavraSorteada;
    }

    public void VerificarSeAcertou()
    {
        if (!PalavraTemCincoLetras())
            return;

        string palpite = ConstruirStringPalpite();

        jogo.ObterPalpite(palpite);

        List<Resultado> resultado = jogo.ConferirPalavra();

        ColorirCelulas(resultado);

        if (EhPraIniciarNovoJogo())
        {
            IniciarJogo();
        }
        else
        {
            jogo.AlterarNumeroJogada();
            AtivarColunas();
        }
    }

    private bool EhPraIniciarNovoJogo()
    {
        if (jogo.Acertou)
        {
            MostrarDialogoNovoJogo(true);
            return true;
        }
        else if (jogo.EhFimDeJogo)
        {
            MostrarDialogoNovoJogo(false);
            return true;
        }
        return false;
    }

    private void MostrarDialogoNovoJogo(bool acertou)
    {
        string message = acertou ? $"-- {jogo.PalavraSorteada.ToUpper()} --\n\nParabéns! Você acertou a palavra.\n\nDeseja iniciar uma nova rodada?" :
            "Suas chances terminaram! Gostaria de jogar novamente?";

        string titulo = acertou ? "\tPalavra Certa" : "\tFim de Jogo";

        var result = MessageBox.Show(message, titulo, MessageBoxButtons.YesNo, MessageBoxIcon.None);

        if (result == DialogResult.No)
        {
            this.Close();
        }
    }

    private void backspace_Click(object sender, EventArgs e)
    {
        int indexAtual = int.Parse(caixaSelecionada.Tag.ToString()!);

        if (caixaSelecionada.Text != "" && indexAtual >= 0)
        {
            caixaSelecionada.Text = "";
            caixaSelecionada.SelectionStart = 0;
            caixaSelecionada.Focus();

        }
        else if (caixaSelecionada.Text == "" && indexAtual > 0)
        {
            caixaSelecionada = BuscarRowJogada().ElementAt(indexAtual - 1);
            caixaSelecionada.SelectionStart = 1;
            caixaSelecionada.Focus();
        }
        else
        {
            BuscarRowJogada().ElementAt(0).Focus();
        }
    }
}