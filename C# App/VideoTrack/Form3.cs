using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VideoTrack
{
    public partial class Form3 : Form
    {
        VideoTrackDataContext db = new VideoTrackDataContext();

        public Form3()
        {
            InitializeComponent();
            foreach (Actor actor in db.Actors)
            {
                comboBoxEdit1.Properties.Items.Add(actor.name);
            }   
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
           
            List<String> toRemoveItems = new List<string>();
            foreach (String item in listBoxControl1.SelectedItems)
            {
                toRemoveItems.Add(item);
            }
            foreach (String item in toRemoveItems)
            {
                listBoxControl1.Items.Remove(item);
            }
        }

        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!listBoxControl1.Items.Contains(comboBoxEdit1.SelectedItem))
                listBoxControl1.Items.Add(comboBoxEdit1.SelectedItem);
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            //var query = from x in db.Movies
            //           join y in db.Movie_Actors on x.movieID equals y.movieID
            //           join z in db.Actors on y.actorID equals z.actorID
            //           group x.Movie_Actors by x.movieID into g
            //           select new { movieID = g.Key, movieActors = g};

            List<string> items = new List<string>();
            for (int i = 0; i < listBoxControl1.Items.Count; i++)
            {
                items.Add(listBoxControl1.Items[i].ToString());               
            }
            get_MoviesContainsSelectedActors(items);


            List<int> l1 = new List<int>(); l1.Add(1); l1.Add(2); l1.Add(3);
            List<int> l2 = new List<int>(); l2.Add(4); l2.Add(1); l2.Add(2);
            List<int> l3 = new List<int>(); l3.Add(2); l3.Add(1);
            intersectMovies(l1, l2, l3);
        }

        private List<int> get_MoviesContainsSelectedActors(List<string> selectedActors)
        {
            List<int> movieIDs = new List<int>();
            var query = db.Movie_Actors.GroupBy(x => x.movieID, x => x.Actor.name, (key, g) => new { movieID = key, Actors = g.ToList()});
            foreach (var group in query)
            {
                if (new HashSet<string>(group.Actors).IsSupersetOf(selectedActors))
                    movieIDs.Add(group.movieID);
            }
            return movieIDs;
        }

        private List<int> get_MoviesContainsSelectedGenres(List<string> selectedGenres)
        {
            List<int> movieIDs = new List<int>();
            var query = db.Movie_Genres.GroupBy(x => x.movieID, x => x.Genre.name, (key, g) => new { movieID = key, Genres = g.ToList() });
            foreach (var group in query)
            {
                if (new HashSet<string>(group.Genres).IsSupersetOf(selectedGenres))
                    movieIDs.Add(group.movieID);
            }
            return movieIDs;
        }

        private List<int> get_MoviesContainsSelectedLangs(List<string> selectedLangs)
        {
            List<int> movieIDs = new List<int>();
            var query = db.Movie_Langs.GroupBy(x => x.movieID, x => x.Lang.name, (key, g) => new { movieID = key, Langs = g.ToList() });
            foreach (var group in query)
            {
                if (new HashSet<string>(group.Langs).IsSupersetOf(selectedLangs))
                    movieIDs.Add(group.movieID);
            }
            return movieIDs;
        }

        private List<int> intersectMovies(List<int> lst1 , List<int> lst2 , List<int> lst3)
        {
            List<int> intersectionResultSet = lst1.Intersect(lst2).Intersect(lst3).ToList();
            return intersectionResultSet;
        }

        private void listBoxControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

