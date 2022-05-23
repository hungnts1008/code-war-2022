
namespace bot {
    public class Player
    {
        public int playerId;
        public string displayName;
        public List<Hero> heroes;
        public HashSet<GemType> heroGemType;
        protected Grid grid;

        public Player(int playerId, string name)
        {
            this.playerId = playerId;
            this.displayName = name;

            heroes = new List<Hero>();
            heroGemType = new HashSet<GemType>();
        }

        public Hero HeroFirstFullMana()
        {
            if(heroes[0].isAlive() && heroes[0].isFullMana())   return heroes[0];
            else return null;
        }
        
        public Hero SecondPlayer()
        {
            if(heroes[1].isAlive() ) return heroes[1];
            else if(heroes[2].isAlive() ) return heroes[2];
            else return firstHeroAlive();
        }

        public Hero HeroHasMaxDamage()
        {
            int dame=0;
            Hero Founded=null;
            foreach (var hero in heroes)
            {
                if(hero.isAlive() &&  hero.attack > dame)
                {
                    dame=hero.attack;
                    Founded=hero;
                }
            }
            return Founded;
        }
        public Hero firstHeroAlive() {
            foreach(var hero in heroes){
                if (hero.isAlive()) return hero;
            }

            return null;
        }

        public Hero IsHeroFullMana(int num)
        {
            if(heroes[num].isFullMana()) return heroes[num];
            else return null;
        }

        public HashSet<GemType> getRecommendGemType() {
            heroGemType.Clear();
            foreach(var hero in heroes){
                if (!hero.isAlive()) continue;
                
                foreach(var gt in hero.gemTypes){
                    heroGemType.Add((GemType)gt);
                }
            }

            return heroGemType;
        }
    }
}