using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using XnaCards;

namespace ProgrammingAssignment6
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const int WINDOW_WIDTH = 800;
        const int WINDOW_HEIGHT = 600;

        // max valid blackjack score for a hand
        const int MAX_HAND_VALUE = 21;

        // deck and hands
        Deck deck;
        List<Card> dealerHand = new List<Card>();
        List<Card> playerHand = new List<Card>();

        // hand placement
        const int TOP_CARD_OFFSET = 100;
        const int HORIZONTAL_CARD_OFFSET = 150;
        const int VERTICAL_CARD_SPACING = 125;

        // messages
        SpriteFont messageFont;
        const string SCORE_MESSAGE_PREFIX = "Score: ";
        Message playerScoreMessage;
        List<Message> messages = new List<Message>();

        // message placement
        const int SCORE_MESSAGE_TOP_OFFSET = 25;
        const int HORIZONTAL_MESSAGE_OFFSET = HORIZONTAL_CARD_OFFSET;
        Vector2 winnerMessageLocation = new Vector2(WINDOW_WIDTH / 2,
            WINDOW_HEIGHT / 2);

        // menu buttons
        Texture2D quitButtonSprite;
        Texture2D hitButtonSprite;
        Texture2D standButtonSprite;
        List<MenuButton> menuButtons = new List<MenuButton>();

        // menu button placement
        const int TOP_MENU_BUTTON_OFFSET = TOP_CARD_OFFSET;
        const int QUIT_MENU_BUTTON_OFFSET = WINDOW_HEIGHT - TOP_CARD_OFFSET;
        const int HORIZONTAL_MENU_BUTTON_OFFSET = WINDOW_WIDTH / 2;
        const int VERTICAL_MENU_BUTTON_SPACING = 125;

        // use to detect hand over when player and dealer didn't hit
        bool playerHit = false;
        bool dealerHit = false;

        // game state tracking
        static GameState currentState = GameState.WaitingForPlayer;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // set resolution and show mouse
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here         
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // create and shuffle deck
            int deckX = 1;
            int deckY = 1;
            Deck deck = new Deck(Content, deckX, deckY);
            deck.Shuffle();
            // first player card
            playerHand.Add(deck.TakeTopCard());
            playerHand[0].FlipOver();
            playerHand[0].X = HORIZONTAL_CARD_OFFSET;
            playerHand[0].Y = TOP_CARD_OFFSET;

            // first dealer card
            dealerHand.Add(deck.TakeTopCard());
            dealerHand[0].FlipOver();
            dealerHand[0].X = WINDOW_WIDTH - HORIZONTAL_CARD_OFFSET;
            dealerHand[0].Y = TOP_CARD_OFFSET;

            // second player card
            playerHand.Add(deck.TakeTopCard());
            playerHand[1].FlipOver();
            playerHand[1].X = HORIZONTAL_CARD_OFFSET;
            playerHand[1].Y = VERTICAL_CARD_SPACING + TOP_CARD_OFFSET;

            // second dealer card
            dealerHand.Add(deck.TakeTopCard());
            dealerHand[1].X = WINDOW_WIDTH - HORIZONTAL_CARD_OFFSET;
            dealerHand[1].Y = VERTICAL_CARD_SPACING + TOP_CARD_OFFSET;

            // load sprite font, create message for player score and add to list
            messageFont = Content.Load<SpriteFont>("Arial24");
            playerScoreMessage = new Message(SCORE_MESSAGE_PREFIX + GetBlackjackScore(playerHand).ToString(),
                messageFont,
                new Vector2(HORIZONTAL_MESSAGE_OFFSET, SCORE_MESSAGE_TOP_OFFSET));
            messages.Add(playerScoreMessage);

            // load quit button sprite for later use
            quitButtonSprite = Content.Load<Texture2D>("quitbutton");

            // create hit button and add to list
            hitButtonSprite = Content.Load<Texture2D>("hitbutton");
            Vector2 hitButtonCenter = new Vector2(HORIZONTAL_MENU_BUTTON_OFFSET, TOP_MENU_BUTTON_OFFSET);
            menuButtons.Add(new MenuButton(hitButtonSprite, hitButtonCenter, GameState.PlayerHitting));

            // create stand button and add to list
            standButtonSprite = Content.Load<Texture2D>("standbutton");
            Vector2 standButtonCenter = new Vector2(HORIZONTAL_MENU_BUTTON_OFFSET, VERTICAL_MENU_BUTTON_SPACING + TOP_MENU_BUTTON_OFFSET);
            menuButtons.Add(new MenuButton(standButtonSprite, standButtonCenter, GameState.WaitingForDealer));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // update menu buttons as appropriate
            MouseState mouse = Mouse.GetState();
            if (currentState == GameState.WaitingForPlayer || currentState == GameState.DisplayingHandResults)
            {
                foreach (MenuButton menuButton in menuButtons)
                {
                    menuButton.Update(mouse);
                }
            }

            // game state-specific processing
            switch (currentState)
            {
                case GameState.PlayerHitting:
                     if ((deck==null)||(deck.TakeTopCard() == null))
                    {  
                        deck = new Deck(Content, 0, 0);
                        deck.Shuffle();
                    }
                    Card nextCard = deck.TakeTopCard();
                    playerHand.Add(nextCard);
                    playerHand.Add(deck.TakeTopCard());
                    playerHand[2].FlipOver();
                    playerHand[2].X = HORIZONTAL_CARD_OFFSET;
                    playerHand[2].Y = (2 * VERTICAL_CARD_SPACING) + TOP_CARD_OFFSET;

                    ChangeState(GameState.WaitingForDealer);
                    playerHit = true;
                    break;
                case GameState.DisplayingHandResults:
                    ChangeState(GameState.Exiting);
                    break;
                case GameState.Exiting:
                    this.Exit();
                    break;
                default:
                    ChangeState(GameState.WaitingForPlayer);
                    break;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Goldenrod);

            spriteBatch.Begin();

            // draw hands
            foreach (Card card in playerHand)
            {
                card.Draw(spriteBatch);
            }
            foreach (Card card in dealerHand)
            {
                card.Draw(spriteBatch);
            }
            // draw messages
            foreach (Message message in messages)
            {
                message.Draw(spriteBatch);
            }
            // draw menu buttons
            foreach (MenuButton menuButton in menuButtons)
            {
                menuButton.Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Calculates the Blackjack score for the given hand
        /// </summary>
        /// <param name="hand">the hand</param>
        /// <returns>the Blackjack score for the hand</returns>
        private int GetBlackjackScore(List<Card> hand)
        {
            // add up score excluding Aces
            int numAces = 0;
            int score = 0;
            foreach (Card card in hand)
            {
                if (card.Rank != Rank.Ace)
                {
                    score += GetBlackjackCardValue(card);
                }
                else
                {
                    numAces++;
                }
            }

            // if more than one ace, only one should ever be counted as 11
            if (numAces > 1)
            {
                score += numAces - 1;
                numAces = 1;
            }

            // if there's an Ace, score it the best way possible
            if (numAces > 0)
            {
                if (score + 11 <= MAX_HAND_VALUE)
                {
                    score += 11;
                }
                else
                {
                    score++;
                }
            }

            return score;
        }

        /// <summary>
        /// Gets the Blackjack value for the given card
        /// </summary>
        /// <param name="card">the card</param>
        /// <returns>the Blackjack value for the card</returns>
        private int GetBlackjackCardValue(Card card)
        {
            switch (card.Rank)
            {
                case Rank.Ace:
                    return 11;
                case Rank.King:
                case Rank.Queen:
                case Rank.Jack:
                case Rank.Ten:
                    return 10;
                case Rank.Nine:
                    return 9;
                case Rank.Eight:
                    return 8;
                case Rank.Seven:
                    return 7;
                case Rank.Six:
                    return 6;
                case Rank.Five:
                    return 5;
                case Rank.Four:
                    return 4;
                case Rank.Three:
                    return 3;
                case Rank.Two:
                    return 2;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Changes the state of the game
        /// </summary>
        /// <param name="newState">the new game state</param>
        public static void ChangeState(GameState newState)
        {
            currentState = newState;
        }
    }
}
