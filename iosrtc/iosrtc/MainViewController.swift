//
//  MainViewController.swift
//  RtcDemo
//
//  Created by fancyjing on 2024/4/11.
//
import UIKit
class MainViewController : UIViewController {
    
    override func viewDidLoad() {
        print("root viewDidLoad")
        super.viewDidLoad()

    }
    
    override func viewDidAppear(_ animated: Bool) {
        print("viewDidAppear")
        super.viewDidAppear(animated)

    }
    
    override func viewWillDisappear(_ animated: Bool) {
        super.viewWillDisappear(animated)
    }
    
    @IBAction func handle1V1RTM(_ sender: UIButton) {
        navTo(name: "RTM1V1")
    }
    
    @IBAction func handleMultiRTM(_ sender: UIButton) {
        navTo(name: "RTMMulti")
    }
    
    @IBAction func handlePullStream(_ sender: UIButton) {

    }
    
    @IBAction func handlePushStream(_ sender: UIButton) {

    }
    
    @IBAction func handleVoip(_ sender: UIButton) {
        navTo(name: "voip")
    }
    
    @IBAction func handleMultiRtc(_ sender: UIButton) {
        navTo(name: "multirtc")
    }
    
    func navTo(name : String) {
        let storyBoard = UIStoryboard(name: "Main", bundle: nil)
        let view = storyBoard.instantiateViewController(withIdentifier: name)as UIViewController
        view.modalPresentationStyle = .fullScreen
        self.present(view, animated: true, completion: nil)
    }
    
    
}
