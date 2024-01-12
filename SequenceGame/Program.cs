namespace SequenceGame
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    public class StartForm : Form
    {
        private Button startGameButton;
        private Color playerColor = Color.Blue; // Default color
        private ComboBox aiSelectionComboBox;
        private int numberOfAIs;

        public StartForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.ClientSize = new Size(220, 200);

            // Color selection buttons
            CreateColorButton("Blue", Color.Blue, new Point(10, 50));
            CreateColorButton("Green", Color.Green, new Point(80, 50));
            CreateColorButton("Red", Color.Red, new Point(150, 50));

            aiSelectionComboBox = new ComboBox();
            aiSelectionComboBox.Items.Add("1 AI");
            aiSelectionComboBox.Items.Add("2 AIs");
            aiSelectionComboBox.SelectedIndex = 0; // Default to 1 AI
            aiSelectionComboBox.Location = new Point(45, 100);
            this.Controls.Add(aiSelectionComboBox);

            // Start game button
            startGameButton = new Button();
            startGameButton.Text = "Start Game";
            startGameButton.Location = new Point(70, 150);
            startGameButton.Click += StartGameButton_Click;
            this.Controls.Add(startGameButton);
        }

        private void CreateColorButton(string text, Color color, Point location)
        {
            Button colorButton = new Button();
            colorButton.Text = text;
            colorButton.BackColor = color;
            colorButton.Location = location;
            colorButton.Size = new Size(60, 30);
            colorButton.Click += (sender, e) => { playerColor = color; };
            this.Controls.Add(colorButton);
        }

        private void StartGameButton_Click(object sender, EventArgs e)
        {
            numberOfAIs = aiSelectionComboBox.SelectedIndex + 1;
            SequenceBoardForm gameForm = new SequenceBoardForm(playerColor, numberOfAIs);
            gameForm.Show();
            this.Hide();
        }
    }

    public class SequenceBoardForm : Form
    {
        private Random random = new Random();

        private const int GridSize = 10;
        private const int CardCount = 6;
        private const int ButtonSizeW = 63;
        private const int ButtonSizeH = 88;

        private Button[,] gridButtons = new Button[GridSize, GridSize];
        private Button[] cardButtons = new Button[CardCount];

        CardDeck deck = new CardDeck();
        private List<string> aiHand;

        private Button? selectedCardButton;

        private Label handLabel;
        private Label lastMovesLabel;

        private string previousMove;
        private Color playerColor;
        private Color AIColor1 = Color.Blue;
        private Color AIColor2 = Color.Red;

        private int numberOfAIs;
        private int currentPlayerTurn;

        private bool secondPlayer = false;

        private int[] sequenceCounts = new int[3];

        public SequenceBoardForm(Color playerColor, int numberOfAIs)
        {
            this.playerColor = playerColor;
            if(playerColor.Name == "Blue")
            {
                AIColor1 = Color.Green;
            }
            if (playerColor.Name == "Red")
            {
                AIColor2 = Color.Green;
            }
            this.numberOfAIs = numberOfAIs;
            currentPlayerTurn = 0;
            InitializeBoard();
            InitializeCardButtons();
            InitializeAiHand();
        }
        private string GetTokenPath(string color)
        {
            string folderPath = @"C:\Users\phili\source\repos\SequenceGame\SequenceGame\CardImages";
            string fileName = color + "_token.png";
            return Path.Combine(folderPath, fileName);
        }
        private string GetImagePath(string cardText)
        {
            string numberName = cardText[0].ToString();
            string suitName = "";
            string styleName = "";

            string folderPath = @"C:\Users\phili\source\repos\SequenceGame\SequenceGame\CardImages";

            switch (cardText[0])
            {
                case 'T':
                    numberName = "10";
                    break;
                case 'J':
                    numberName = "jack";
                    styleName = "2";
                    break;
                case 'Q':
                    numberName = "queen";
                    styleName = "2";
                    break;
                case 'K':
                    numberName = "king";
                    styleName = "2";
                    break;
                case 'A':
                    numberName = "ace";
                    break;
                case 'F':
                    numberName = "free";
                    break;
            }

            switch (cardText[1]) 
            {
                case 'H':
                    suitName = "hearts";
                    break;
                case 'D':
                    suitName = "diamonds";
                    break;
                case 'C':
                    suitName = "clubs";
                    break;
                case 'S':
                    suitName = "spades";
                    break;
            }

            string fileName = numberName + "_of_" + suitName + styleName + ".png";

            return Path.Combine(folderPath, fileName);
        }
        private void InitializeBoard()
        {
            this.ClientSize = new Size(GridSize * ButtonSizeW + 20, GridSize * (ButtonSizeH + 20));

            string[,] layout = new string[,] {
            { "Free", "6D", "7D", "8D", "9D", "TD", "QD", "KD", "AD", "Free" },
            { "5D", "3H", "2H", "2S", "3S", "4S", "5S", "6S", "7S", "AC" },
            { "4D", "4H", "KD", "AD", "AC", "KC", "QC", "TC", "8S", "KC" },
            { "3D", "5H", "QD", "QH", "TH", "9H", "8H", "9C", "9S", "QC" },
            { "2D", "6H", "TD", "KH", "3H", "2H", "7H", "8C", "TS", "TC" },
            { "AS", "7H", "9D", "AH", "4H", "5H", "6H", "7C", "QS", "9C" },
            { "KS", "8H", "8D", "2C", "3C", "4C", "5C", "6C", "KS", "8C" },
            { "QS", "9H", "7D", "6D", "5D", "4D", "3D", "2D", "AS", "7C" },
            { "TS", "TH", "QH", "KH", "AH", "2C", "3C", "4C", "5C", "6C" },
            { "Free", "9S", "8S", "7S", "6S", "5S", "4S", "3S", "2S", "Free" }
            };

            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    var button = new Button
                    {
                        Size = new Size(ButtonSizeW, ButtonSizeH),
                        Location = new Point(i * ButtonSizeW + 10, j * ButtonSizeH + 10),
                        Name = layout[i, j],
                        BackColor = Color.White,
                        ForeColor = Color.White
                    };
                    button.Click += BoardButton_Click;
                    gridButtons[i, j] = button;
                    this.Controls.Add(button);
                }
            }

            foreach (Button gridButtons in gridButtons)
            {
                try
                {
                    string imagePath = GetImagePath(gridButtons.Name);
                    gridButtons.BackgroundImage = Image.FromFile(imagePath);
                    gridButtons.BackgroundImageLayout = ImageLayout.Stretch;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image: {ex.Message}");
                }
            }
        }
        private void InitializeCardButtons()
        {
            int spacing = 10;
            int startY = GridSize * ButtonSizeH + 30;
            int startX = (this.ClientSize.Width - (CardCount * ButtonSizeW + (CardCount - 1) * spacing)) / 2;

            handLabel = new Label
            {
                Text = "Player Color: " + playerColor.Name,
                Location = new Point(10, startY - 20),
                AutoSize = true
            };
            this.Controls.Add(handLabel);

            lastMovesLabel = new Label
            {
                Text = "Last moves: ",
                Location = new Point(this.ClientSize.Width - 120, startY - 20),
                AutoSize = true
            };
            this.Controls.Add(lastMovesLabel);

            for (int i = 0; i < CardCount; i++)
            {
                var cardButton = new Button
                {
                    Size = new Size(ButtonSizeW, ButtonSizeH),
                    Location = new Point(startX + i * (ButtonSizeW + spacing), startY),
                    Name = deck.DrawCard(),
                    BackColor = Color.White,
                    ForeColor = Color.White
                };
                cardButton.Click += CardButton_Click;
                cardButtons[i] = cardButton;
                this.Controls.Add(cardButton);
            }

            foreach (Button cardButton in cardButtons)
            {
                try
                {
                    string imagePath = GetImagePath(cardButton.Name);
                    cardButton.BackgroundImage = Image.FromFile(imagePath);
                    cardButton.BackgroundImageLayout = ImageLayout.Stretch;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image: {ex.Message}");
                }
            }
        }
        private void InitializeAiHand()
        {
            aiHand = new List<string>();

            for (int i = 0; i < CardCount; i++)
            {
                aiHand.Add(deck.DrawCard());
            }
        }
        private void UpdateLastMoves(string Move)
        {
            lastMovesLabel.Text = "Last moves: " + Move + ", " + previousMove;
            previousMove = Move;
        }
        public class CardDeck
        {
            private readonly Random random = new Random();
            private readonly Dictionary<string, int> cardCounts = new Dictionary<string, int>();
            private readonly string[] suits = { "H", "D", "C", "S" };
            private readonly string[] ranks = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "T", "J", "Q", "K" };

            public CardDeck()
            {
                foreach (var suit in suits)
                {
                    foreach (var rank in ranks)
                    {
                        cardCounts[rank + suit] = 2;
                    }
                }
            }

            public string DrawCard()
            {
                List<string> availableCards = new List<string>(cardCounts.Keys);
                string card;
                do
                {
                    if (availableCards.Count == 0)
                    {
                        throw new InvalidOperationException("No more cards to draw.");
                    }
                    card = availableCards[random.Next(availableCards.Count)];
                } while (cardCounts[card] == 0);

                cardCounts[card] -= 1;

                return card;
            }
        }
        private void EndTurn(Color playColor)
        {
            if (CheckForSequence(playColor))
            {
                sequenceCounts[currentPlayerTurn]++;
                int sequencesNeeded = numberOfAIs == 1 ? 2 : 1;

                if (sequenceCounts[currentPlayerTurn] >= sequencesNeeded)
                {
                    string message = currentPlayerTurn == 0 ? "Player wins!" : $"AI {currentPlayerTurn} wins!";
                    ShowEndGameDialog(message);
                    return;
                }
            }

            currentPlayerTurn = (currentPlayerTurn + 1) % (numberOfAIs + 1);

            if (currentPlayerTurn > 0)
            {
                AiTurn(currentPlayerTurn);
            }
        }
        private void ShowEndGameDialog(string message)
        {
            var result = MessageBox.Show(message + "\nDo you want to play again?", "Game Over", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                ResetGameState();
            }
            else
            {
                this.Close();
            }
        }
        private void ResetGameState()
        {
            // Reset the game state for a new game
        }

        private void PlayerMove(Button selectedCardButton, Button boardButton)
        {
            if (selectedCardButton.Name == "JH" || selectedCardButton.Name == "JS")
            {
                boardButton.Image = null;
                boardButton.BackColor = Color.White;
            }
            else
            {
                try
                {
                    string imagePath = GetTokenPath(playerColor.Name);
                    boardButton.Image = Image.FromFile(imagePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image: {ex.Message}");
                }

                boardButton.BackColor = playerColor;
            }

            UpdateLastMoves(selectedCardButton.Name);

            selectedCardButton.Name = deck.DrawCard();

            try
            {
                string imagePath = GetImagePath(selectedCardButton.Name);
                selectedCardButton.BackgroundImage = Image.FromFile(imagePath);
                selectedCardButton.BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}");
            }

            secondPlayer = false;

            EndTurn(playerColor);
        }
        private void AiTurn(int aiNumber)
        {
            string aiSelectedCard = SelectCardForAI();

            if (aiSelectedCard == "JH" || aiSelectedCard == "JS")
            {
                (int, int) boardPosition = FindBestChipToRemove();
                if (boardPosition != (-1, -1))
                {
                    Button boardButton = gridButtons[boardPosition.Item1, boardPosition.Item2];

                    boardButton.Image = null;
                    boardButton.BackColor = Color.White;
                }
            }
            else
            {
                (int, int) boardPosition = FindValidBoardPosition(aiSelectedCard);
                if (boardPosition != (-1, -1))
                {
                    Button boardButton = gridButtons[boardPosition.Item1, boardPosition.Item2];

                    try
                    {
                        if (aiNumber == 1)
                        {
                            string imagePath = GetTokenPath(AIColor1.Name);
                            boardButton.Image = Image.FromFile(imagePath);
                            boardButton.BackColor = AIColor1;
                        }
                        else
                        {
                            string imagePath = GetTokenPath(AIColor2.Name);
                            boardButton.Image = Image.FromFile(imagePath);
                            boardButton.BackColor = AIColor2;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading image: {ex.Message}");
                    }
                }
            }

            UpdateLastMoves(aiSelectedCard);

            if(aiNumber == 2 && secondPlayer == true)
            {
                EndTurn(AIColor2);
            }
            else
            {
                secondPlayer = true;
                EndTurn(AIColor1);
            }
        }
        private string SelectCardForAI()
        {
            int selectedCardIndex = random.Next(aiHand.Count);

            string selectedCard = aiHand[selectedCardIndex];

            aiHand[selectedCardIndex] = deck.DrawCard();
            
            return selectedCard;
        }
        private (int, int) FindValidBoardPosition(string card)
        {
            List<(int, int)> validPositions = new List<(int, int)>();

            for (int i = 0; i < gridButtons.GetLength(0); i++)
            {
                for (int j = 0; j < gridButtons.GetLength(1); j++)
                {
                    if ((gridButtons[i, j].Name == card || card == "JD" || card == "JC") && gridButtons[i, j].BackColor != playerColor && gridButtons[i, j].BackColor != AIColor1 && gridButtons[i, j].BackColor != AIColor2 && gridButtons[i, j].BackColor != Color.Yellow)
                    {
                        validPositions.Add((i, j));
                    }
                }
            }

            if (validPositions.Count > 0)
            {
                return validPositions[random.Next(validPositions.Count)];
            }

            return (-1, -1);
        }
        private (int, int) FindBestChipToRemove()
        {
            List<(int, int)> opponentChips = new List<(int, int)>();

            for (int i = 0; i < gridButtons.GetLength(0); i++)
            {
                for (int j = 0; j < gridButtons.GetLength(1); j++)
                {
                    if (gridButtons[i, j].BackColor == playerColor)
                    {
                        opponentChips.Add((i, j));
                    }
                }
            }

            if (opponentChips.Count > 0)
            {
                return opponentChips[random.Next(opponentChips.Count)];
            }

            return (-1, -1);
        }
        private bool CheckForSequence(Color playerColor)
        {
            for (int i = 0; i < gridButtons.GetLength(0); i++)
            {
                for (int j = 0; j < gridButtons.GetLength(1); j++)
                {
                    if (CheckHorizontalSequence(i, j, playerColor) ||
                        CheckVerticalSequence(i, j, playerColor) ||
                        CheckDiagonalSequence(i, j, playerColor) ||
                        CheckReverseDiagonalSequence(i, j, playerColor))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private bool CheckSequence(int startX, int startY, int stepX, int stepY, Color playerColor)
        {
            int inSequence = 0;
            Button[] inSequenceChips = new Button[5];

            for (int i = 0; i < 5; i++)
            {
                int x = startX + (i * stepX);
                int y = startY + (i * stepY);
                if (x < 0 || y < 0 || x >= gridButtons.GetLength(0) || y >= gridButtons.GetLength(1))
                    return false;

                Color chipColor = gridButtons[x, y].BackColor;

                if (chipColor == playerColor || IsCorner(x, y))
                {
                    inSequenceChips[i] = gridButtons[x, y];
                    inSequence++;
                }
                else
                {
                    break;
                }
            }
            if(inSequence >= 5)
            {
                for(int j = 0; j < inSequenceChips.Length; j++)
                {
                    try
                    {
                        string imagePath = GetTokenPath("Yellow");
                        inSequenceChips[j].Image = Image.FromFile(imagePath);
                        inSequenceChips[j].BackColor = Color.Yellow;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading image: {ex.Message}");
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool CheckHorizontalSequence(int x, int y, Color playerColor)
        {
            return CheckSequence(x, y, 1, 0, playerColor);
        }
        private bool CheckVerticalSequence(int x, int y, Color playerColor)
        {
            return CheckSequence(x, y, 0, 1, playerColor);
        }
        private bool CheckDiagonalSequence(int x, int y, Color playerColor)
        {
            return CheckSequence(x, y, 1, 1, playerColor);
        }
        private bool CheckReverseDiagonalSequence(int x, int y, Color playerColor)
        {
            return CheckSequence(x, y, -1, 1, playerColor);
        }
        private bool IsCorner(int x, int y)
        {
            int maxIndex = gridButtons.GetLength(0) - 1;
            return (x == 0 || x == maxIndex) && (y == 0 || y == maxIndex);
        }
        private bool IsValidMove(Button cardButton, Button boardButton)
        {
            if (cardButton.Name == "JH" || cardButton.Name == "JS")
            {
                return (boardButton.BackColor == AIColor1 || boardButton.BackColor == AIColor2);
            }
            else
            {
                return (boardButton.Name == cardButton.Name || cardButton.Name == "JD" || cardButton.Name == "JC") && boardButton.BackColor != playerColor && boardButton.BackColor != AIColor1 && boardButton.BackColor != AIColor2 && boardButton.BackColor != Color.Yellow;
            }
        }
        private void CardButton_Click(object sender, EventArgs e)
        {
            Button cardButton = sender as Button;
            if (cardButton != null)
            {
                selectedCardButton = cardButton;
            }

            if (selectedCardButton != null)
            {
                HighlightMatchingBoardButtons(selectedCardButton, highlight: false);
                selectedCardButton = null;
            }

            if (selectedCardButton != cardButton)
            {
                selectedCardButton = cardButton;
                HighlightMatchingBoardButtons(selectedCardButton, highlight: true);
            }
        }
        private void BoardButton_Click(object sender, EventArgs e)
        {
            var boardButton = sender as Button;

            if (selectedCardButton != null && boardButton != null && IsValidMove(selectedCardButton, boardButton))
            {
                PlayerMove(selectedCardButton, boardButton);
            }
        }
        private void HighlightMatchingBoardButtons(Button cardButton, bool highlight)
        {
            foreach (Button boardButton in gridButtons)
            {
                if (boardButton.Name == cardButton.Name)
                {
                    if (highlight)
                    {
                        // Highlight the button (for example, with a glowing border)
                        boardButton.FlatAppearance.BorderSize = 2;
                        boardButton.FlatAppearance.BorderColor = Color.Blue;
                    }
                    else
                    {
                        // Remove the highlight
                        boardButton.FlatAppearance.BorderSize = 0;
                    }
                }
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new StartForm());
        }
    }

}