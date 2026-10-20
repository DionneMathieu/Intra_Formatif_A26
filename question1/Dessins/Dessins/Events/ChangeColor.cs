namespace Dessins.Events
{
    // NOUVEAU (exercice 7) : event "ChangeColor" pour changer la couleur du crayon
    // entre deux formes (carrés en rouge, étoile en jaune).
    public class ChangeColor : DrawingEvent
    {
        public override string Type { get { return "ChangeColor"; } }
        public string Color { get; set; }

        public ChangeColor(string color)
        {
            Color = color;
        }
    }
}
