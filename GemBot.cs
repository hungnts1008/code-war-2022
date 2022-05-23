using Sfs2X.Entities;
using Sfs2X.Entities.Data;
namespace bot
{
    public class GemBot : BaseBot
    {
        internal void Load()
        {
            Console.WriteLine("Bot.Load()");
        }

        internal void Update(TimeSpan gameTime)
        {
            Console.WriteLine("Bot.Update()");
        }

        protected override void StartGame(ISFSObject gameSession, Room room)
        {
            // Assign Bot player & enemy player
            AssignPlayers(room);

            // Player & Heroes
            ISFSObject objBotPlayer = gameSession.GetSFSObject(botPlayer.displayName);
            ISFSObject objEnemyPlayer = gameSession.GetSFSObject(enemyPlayer.displayName);

            ISFSArray botPlayerHero = objBotPlayer.GetSFSArray("heroes");
            ISFSArray enemyPlayerHero = objEnemyPlayer.GetSFSArray("heroes");

            for (int i = 0; i < botPlayerHero.Size(); i++)
            {
                var hero = new Hero(botPlayerHero.GetSFSObject(i));
                botPlayer.heroes.Add(hero);
            }

            for (int i = 0; i < enemyPlayerHero.Size(); i++)
            {
                enemyPlayer.heroes.Add(new Hero(enemyPlayerHero.GetSFSObject(i)));
            }

            // Gems
            grid = new Grid(gameSession.GetSFSArray("gems"), botPlayer.getRecommendGemType());
            currentPlayerId = gameSession.GetInt("currentPlayerId");
            log("StartGame ");

            // SendFinishTurn(true);
            //taskScheduler.schedule(new FinishTurn(true), new Date(System.currentTimeMillis() + delaySwapGem));
            TaskSchedule(delaySwapGem, _ => SendFinishTurn(true));
        }

        protected override void SwapGem(ISFSObject paramz)
        {
            bool isValidSwap = paramz.GetBool("validSwap");
            if (!isValidSwap)
            {
                return;
            }

            HandleGems(paramz);
        }

        protected override void HandleGems(ISFSObject paramz)
        {
            ISFSObject gameSession = paramz.GetSFSObject("gameSession");
            currentPlayerId = gameSession.GetInt("currentPlayerId");
            //get last snapshot
            ISFSArray snapshotSfsArray = paramz.GetSFSArray("snapshots");
            ISFSObject lastSnapshot = snapshotSfsArray.GetSFSObject(snapshotSfsArray.Size() - 1);
            bool needRenewBoard = paramz.ContainsKey("renewBoard");
            // update information of hero
            HandleHeroes(lastSnapshot);
            if (needRenewBoard)
            {
                grid.updateGems(paramz.GetSFSArray("renewBoard"));
                TaskSchedule(delaySwapGem, _ => SendFinishTurn(false));
                return;
            }
            // update gem
            grid.gemTypes = botPlayer.getRecommendGemType();
            grid.updateGems(lastSnapshot.GetSFSArray("gems"));
            TaskSchedule(delaySwapGem, _ => SendFinishTurn(false));
        }

        private void HandleHeroes(ISFSObject paramz)
        {
            ISFSArray heroesBotPlayer = paramz.GetSFSArray(botPlayer.displayName);
            for (int i = 0; i < botPlayer.heroes.Count; i++)
            {
                botPlayer.heroes[i].updateHero(heroesBotPlayer.GetSFSObject(i));
            }

            ISFSArray heroesEnemyPlayer = paramz.GetSFSArray(enemyPlayer.displayName);
            for (int i = 0; i < enemyPlayer.heroes.Count; i++)
            {
                enemyPlayer.heroes[i].updateHero(heroesEnemyPlayer.GetSFSObject(i));
            }
        }

        protected override void StartTurn(ISFSObject paramz)
        {
            currentPlayerId = paramz.GetInt("currentPlayerId");
            if (!isBotTurn())
            {
                return;
            }
            Hero FirstFull=botPlayer.HeroFirstFullMana();
            if(FirstFull != null)
            {
                TaskSchedule(delaySwapGem, _ => SendCastSkill(FirstFull));
                grid.temp++;
                return;
            }
            else
            {
                if(grid.CheckSkillorSwap() == true)
                {
                    TaskSchedule(delaySwapGem, _ => SendSwapGem());
                    return;
                }
                else
                {
                    Hero CheckHero2 = botPlayer.IsHeroFullMana(1);
                    if(CheckHero2 != null)  TaskSchedule(delaySwapGem, _ => SendCastSkill(CheckHero2));
                    else
                    {
                        Hero CheckHero3 = botPlayer.IsHeroFullMana(2);
                        if(CheckHero3 != null)
                        {
                            if(enemyPlayer.HeroHasMaxDamage()!=null && enemyPlayer.HeroHasMaxDamage().attack > 10 
                                && enemyPlayer.HeroHasMinHealth()!=null && enemyPlayer.HeroHasMinHealth().hp <= 20)   TaskSchedule(delaySwapGem, _ => SendCastSkill(CheckHero3));
                            else TaskSchedule(delaySwapGem, _ =>SendSwapGem());
                        }
                        else TaskSchedule(delaySwapGem,_ =>SendSwapGem());
                    }
                }
            }
        }

        protected bool isBotTurn()
        {
            return botPlayer.playerId == currentPlayerId;
        }
    }
}