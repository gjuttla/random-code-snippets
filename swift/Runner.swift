protocol Runnable {
    init()
    func run()
}

class Hello : Runnable {
    required init() {}
    func run() {
        print("Hello::\(#function)")
    }
}

class HelloOther : Runnable {
    required init() {}
    func run() {
        print("Hello from the other side")
    }
}

class Runner {
    static func run<T: Runnable>(_ someClass: T.Type) {
        let runnable = someClass.init()
        runnable.run()
    }
}

Runner.run(HelloOther.self)