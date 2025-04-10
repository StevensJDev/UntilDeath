#!/bin/python3

import math
import os
import random
import re
import sys



#
# Complete the 'getProductSuggestions' function below.
#
# The function is expected to return a 2D_STRING_ARRAY.
# The function accepts following parameters:
#  1. STRING_ARRAY products
#  2. STRING search
#

def getProductSuggestions(products, search):
    # Sort the products alphabetically
    products.sort()
    
    # Initialize the result list
    result = []
    
    # Iterate through each character in the search word
    for i in range(1, len(search) + 1):
        # Get the current prefix
        prefix = search[:i]
        
        # Find all products that start with the current prefix
        suggestions = [product for product in products if product.startswith(prefix)]
        
        # Limit the suggestions to the first three products
        suggestions = suggestions[:3]
        
        # Append the suggestions to the result list
        result.append(suggestions)
    
    return result

if __name__ == '__main__':
    fptr = open(os.environ['OUTPUT_PATH'], 'w')

    products_count = int(input().strip())

    products = []

    for _ in range(products_count):
        products_item = input()
        products.append(products_item)

    search = input()

    result = getProductSuggestions(products, search)

    fptr.write('\n'.join([' '.join(x) for x in result]))
    fptr.write('\n')

    fptr.close()
