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

        public bool HasAnyBotFullMana()
        {
            return (heroes[2].isFullMana());
        }
        public bool HeroAlive(int Num)
        {
            return heroes[Num].isAlive();
        }
        public Hero HeroHasMinHealth()
        {
            int health = 100;
            Hero res=null;
            foreach (var hero in heroes)
            {
                if(hero.hp < health) 
                {
                    health = hero.hp;
                    res=hero;
                }
            }
            return res;
        }
        
        public bool HasAnyHeroFullMana()
        {
            foreach(var hero in heroes)
            {
                if(hero.isFullMana())   return true;
            }
            return false;
        }
        public Hero HeroHasMaxDamage()
        {
            Hero Founded=null;
            foreach (var hero in heroes)
            {
                if(hero.isAlive())
                {
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

        public int DameOfFirstAlive()
        {
            int res=0;
            foreach(var hero in heroes)
            {
                if(hero.isAlive())
                {
                    res=hero.attack;
                    break;
                }
            }
            return res;
        }
    }
}