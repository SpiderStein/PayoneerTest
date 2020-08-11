# Balance Manager Exercise

## Create the following APIs:
1.  Create Balance
    - Input: N/A
    - Output: The id of the new balance, id supposed to work as a sequence always the return the next number. For example if the currently the max id is 7 the new balance will be created with id 8.
    
2.  Get Balance Info
    - Input: Balance id
    - Output: 
      - Balance Details
      - If balance doesn't exists return NotFound
      
3. Charge
    - Input: Balance id, amount to charge
    - Output: 
      - If balance doesn't have sufficient amount return BadRequest with relevant message, 
      - If balance doesn't exist return BadRequest with relevant message
      - If it has enough substract the amount from it's balance and return OK
                
4. Load 
    - Input: Balance id, amount to load
    - Output: 
      - If balance doesn't exist return BadRequest with relevant message
      - Else add the amount to it's balance and return OK
                
5. Transfer 
      - Input: Balance id from, Balance id to, amount to transfer
      - Output: 
          - If either of the balances doesn't exist return BadRequest with relevant message,
          - If balance doesn't have sufficient amount to transfer from return BadRequest with relevant message,
          - If it has enough decrease the amount from it's balance and increase amount of receiver balance return OK
                
## Pay attention 
1. Feel Free to create any other classes you may need
2. Every transaction should be completed or failed as a whole, not partially completed!
3. Make sure your code is supporting parallel request and is thread safe
4. DAL project is not for you to change :)
