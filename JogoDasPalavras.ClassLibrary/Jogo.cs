namespace JogoDasPalavras.ClassLibrary
{
    public partial class Jogo
    {
        public int NumeroJogada { get; private set; }

        public string PalavraSorteada { get; private set; }

        public string? PalavraPalpite { get; private set; }

        private readonly List<int> ListaDeRetorno;

        public bool Acertou { get { return PalavraPalpite == PalavraSorteada; } }

        public bool EhFimDeJogo { get { return NumeroJogada == 4; } }

        public Jogo()
        {
            this.PalavraSorteada = ObterPalavara();
            this.NumeroJogada = 0;
            ListaDeRetorno = new List<int>();

        }
        private string ObterPalavara()
        {
            return Palavras[new Random().Next(Palavras.Length)];
        }

        public void AlterarNumeroJogada()
        {
            this.NumeroJogada++;
        }

        public void ObterPalpite(string palavra)
        {
            this.PalavraPalpite = palavra.ToLower();
        }

        public List<int> ConferirPalavra()
        {
            ListaDeRetorno.Clear();

            for (int i = 0; i < this.PalavraSorteada.Length; i++)
            {
                if (PalavraPalpite![i] == PalavraSorteada[i])
                {
                    this.ListaDeRetorno.Add(1);
                }
                else if (PalavraSorteada.Contains(PalavraPalpite[i]))
                {
                    this.ListaDeRetorno.Add(0);
                }
                else
                {
                    this.ListaDeRetorno.Add(-1);
                }
            }

            return ListaDeRetorno;
        }
    }
}
