using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Script
{
    public class FillNode : MonoBehaviour
    {
        public bool isFilled;

      
        
        private void OnTriggerStay2D(Collider2D other)
        {
            
         
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("obstacle"))
            {
                isFilled = true;
               
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
        
            if (other.CompareTag("Player") || other.CompareTag("obstacle"))
            {
                isFilled = false;
                
            }
        }
    }
}