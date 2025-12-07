// Global variable
int global_counter = 0;

// Function prototype
void increment_counter(int ,int );

int main() {
    int x = 5;
    float y = 3.14;
    char c = 'A';    
    // Arithmetic operations
    x = x + y * 2;
    x++;
    
    // Control flow
    if (x > 10 && y < 5.0) {
        printf("x is large: %d\n", x);
    } else {
        printf("x is small: %d\n", x);
    }
    
    // Loop
    for (int i = 0; i < 5; i++) {
        printf("Iteration %d\n", i);
    }

    // Function call
    increment_counter(10);
    
    return 0;
}

void increment_counter(int) {
    global_counter += amount;
    printf("Counter: %d\n", global_counter);
}