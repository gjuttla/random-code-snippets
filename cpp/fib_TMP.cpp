#include <iostream>

using namespace std;

template <int n>
struct fib {
    static const int val = fib<n - 1>::val + fib<n - 2>::val;
};

template<>
struct fib<0> {
    static const int val = 0;
};

template<>
struct fib<1> {
    static const int val = 1;
};

int main(int argc, char* argv[]) {
    cout << fib<10>::val << endl;
    return 0;
}