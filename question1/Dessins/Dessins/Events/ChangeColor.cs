namespace Dessins.Events
{
    public class ChangeColor : DrawingEvent
    {
        public override string Type { get { return "Couleur"; } }
        public string Couleur { get; set; }
        public ChangeColor(string couleur)
        {
            Couleur = couleur;
        }


    }
}
