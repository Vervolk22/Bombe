using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnigmaCryptography
{
    /// <summary>
    /// Represents a single rotor in Enigma machine.
    /// </summary>
    public class Rotor
    {
        protected string layout;
        protected byte offset;
        protected Rotor previous, next;
        protected char cIn = '\0', notchPos;
        private byte savedOffset;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="layout">Commutation layout of rotor.</param>
        /// <param name="notchPos">Notch position at this rotor in 
        /// Enigma machine.</param>
        public Rotor(string layout, char notchPos)
        {
            this.layout = layout;
            this.notchPos = notchPos;
            offset = 0;

        }

        /// <summary>
        /// Gets rotor's commutation layout.
        /// </summary>
        /// <returns>Commutation layout.</returns>
        public string GetLayout()
        {
            return layout;
        }

        /// <summary>
        /// Sets next rotor in Enigma machine.
        /// </summary>
        /// <param name="next">Next rotor.</param>
        public void SetNextRotor(Rotor next)
        {
            this.next = next;
        }
        /// <summary>
        /// Sets previous rotor in Enigma machine.
        /// </summary>
        /// <param name="previous">Previous rotor.</param>
        public void SetPreviousRotor(Rotor previous)
        {
            this.previous = previous;
        }

        /// <summary>
        /// Encrypts char, when it goes back to the first rotor
        /// from the reflector.
        /// </summary>
        /// <param name="ch">Char to encrypt.</param>
        /// <returns>Encrypted char by the current rotor.</returns>
        public char GetInverseCharAt(string ch)
        {
            int pos = layout.IndexOf(ch);

            if (offset > pos)
            {
                pos = 26 - (offset - pos);
            }
            else
            {
                pos = pos - offset;
            }

            if (previous != null)
            {
                pos = (pos + previous.GetOffset()) % 26;
            }

            return (char)(65 + pos);
        }

        /// <summary>
        /// Get the current rotor's offset.
        /// </summary>
        /// <returns>Rotor's offset.</returns>
        public byte GetOffset()
        {
            return offset;
        }

        /// <summary>
        /// Get the current rotor's notch position.
        /// </summary>
        /// <returns>Rotor's notch position.</returns>
        public char GetNotchPos()
        {
            return notchPos;
        }

        /// <summary>
        /// Resets rotor's offset to zero.
        /// </summary>
        public void ResetOffset()
        {
            offset = 0;
        }

        /// <summary>
        /// Checks, if the current rotor has a next rotor.
        /// </summary>
        /// <returns>Result of check.</returns>
        public bool HasNext()
        {
            return next != null;
        }

        /// <summary>
        /// Checks, if the current rotor has a previous rotor.
        /// </summary>
        /// <returns>Result of check.</returns>
        public bool HasPrevious()
        {
            return previous != null;
        }

        /// <summary>
        /// Move the current rotor by 1 position ahead, and, if it is in notch
        /// position, move the next rotor too.
        /// </summary>
        public void Move()
        {
            // Reflector can't change it's offset.
            if (next == null)
            {
                return;
            }
            offset++;
            if (offset == 26)
            {
                offset = 0;
            }

            if (next != null && (offset + 66) == ((notchPos - 64) % 26) + 66)
            {
                next.Move();
            }
        }

        /// <summary>
        /// Move the current rotor by 1 position backwards, if it is in notch
        /// position, move the next rotor too. It can be used only while
        /// breaking encrypted messages.
        /// </summary>
        public void MoveBack()
        {
            if (next != null && (offset + 66) == ((notchPos - 64) % 26) + 66)
            {
                next.MoveBack();
            }

            if (offset == 0)
            {
                offset = 26;
            }
            offset--;
        }

        /// <summary>
        /// Puts char in the current rotor, encrypt it, and send to the next
        /// rotor.
        /// </summary>
        /// <param name="s">Char to encrypt.</param>
        public void PutDataIn(char s)
        {
            cIn = s;
            char c = s;
            c = (char)(((c - 65) + offset) % 26 + 65);

            if (next != null)
            {
                c = layout.Substring((c - 65), 1).ToCharArray()[0];
                if ((((c - 65) + (-offset)) % 26 + 65) >= 65)
                {
                    c = (char)(((c - 65) + (-offset)) % 26 + 65);
                }
                else
                {
                    c = (char)(((c - 65) + (26 + (-offset))) % 26 + 65);
                }
                next.PutDataIn(c);
            }
        }

        /// <summary>
        /// Gets char from the next rotor, encrypt it, and send to the 
        /// previous rotor.
        /// </summary>
        /// <returns>Encrypted char.</returns>
        public char GetDataOut()
        {
            char c = '\0';

            if (next != null)
            {
                c = next.GetDataOut();
                c = GetInverseCharAt("" + c);
            }
            else
            {
                // Only in reflector case.
                c = layout.Substring((cIn - 65), 1).ToCharArray()[0];
                c = (char)(((c - 65) + previous.offset) % 26 + 65);
            }

            return c;
        }

        /// <summary>
        /// Saves the current rotor's offset locally.
        /// </summary>
        public void saveOffset()
        {
            savedOffset = offset;
            if (next != null) next.saveOffset();
        }

        /// <summary>
        /// Restore the saved offset from local storage.
        /// </summary>
        public void restoreOffset()
        {
            offset = savedOffset;
            if (next != null) next.restoreOffset();
        }

        /// <summary>
        /// Set the current rotor's offset to the given value.
        /// </summary>
        /// <param name="offset">New rotor's offset.</param>
        public void setOffset(byte offset)
        {
            this.offset = offset;
        }
    }

}
