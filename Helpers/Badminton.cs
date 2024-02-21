using System.ComponentModel;

namespace MARCUS.Helpers
{
    public class Badminton : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _Fatto = 0;
        public int Fatto
        {
            get { return _Fatto; }
            set
            {
                _Fatto = value;
                OnPropertyChanged(nameof(Fatto));
            }
        }

        private int _Scarto = 0;
        public int Scarto
        {
            get { return _Scarto; }
            set
            {
                _Scarto = value;
                OnPropertyChanged(nameof(Scarto));
            }
        }

        private int _Sospeso = 0;
        public int Sospeso
        {
            get { return _Sospeso; }
            set
            {
                _Sospeso = value;
                OnPropertyChanged(nameof(Sospeso));
            }
        }

        private string _Riferimento = "";
        public string Riferimento
        {
            get { return _Riferimento; }
            set
            {
                _Riferimento = value;
                OnPropertyChanged(nameof(Riferimento));
            }
        }
    }
}