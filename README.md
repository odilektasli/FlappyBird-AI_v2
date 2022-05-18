# FlappyBird-AI_v2
### Unity project that trains FlappyBird agent using neural network and genetic algorithm


From MainMenu scene, population size can be edit

![resim](https://user-images.githubusercontent.com/61044813/124247635-ba8f6080-db2a-11eb-8879-8ade8fcd8445.png)

After declaration birds are ready to learn with the followings.

![resim](https://user-images.githubusercontent.com/61044813/124247875-faeede80-db2a-11eb-9b39-1d7dd8a0bef1.png)


![flappyBirdAI](https://user-images.githubusercontent.com/61044813/169121275-e1f3a290-c1ea-4122-a1e4-ad293f6824bf.PNG)

Creation of a herd that will learn how to be successful. Also, the herd (birds) can access only the data that humans can obtain which are:
_a.	Destination between bird and closest pipe
b.	Height of the bird on Y coordinate
c.	Height of the pipe on Y coordinate_


In the application we used Unity with Genetic Algorithm and Neural Network. To teach the birds how they can fly, we used the Neural Network with the layers of 3-6-3-1. In the Neural Network for every single bird; get inputs for the first layer’s nodes with their weights. Then creates output for the next layer and the same step for the single output. With Genetic Algorithm we transfer the individual nodes to the next generation, but with some changes.

![resim](https://user-images.githubusercontent.com/61044813/124250216-54f0a380-db2d-11eb-8a93-01cff08452a0.png)


_**a.	Movement Of The Birds**_

Birds move with the calculated -/+ velocity (output) on the Y axis. To create outputs (creation of weights) in the Neural Network tanh function is used. It is provided to controlling of the outputs, so clamps the output weights. Value calculation is that multiplication by their weights and getting the output between -1 and 1 thanks to tanh function. But output comes from tanh is not used directly. We are subtracting “bias” from the output. Bias is kind of virtual layer on the hidden layers. It is used for the noise cancelling, to reduce incoming nonsense values. In another saying Bias provides pruning to approach better solution faster.

**_b.	Roulette Wheel Selection_**

![resim](https://user-images.githubusercontent.com/61044813/124250174-473b1e00-db2d-11eb-89a8-02a60a948c49.png)


Firstly, we get the best bird (that has the best score in the previous generation) as an elite one and clone it to 16 birds. Then we select the remaining birds for the next generation with wheel selection. For the remaining birds in the generation, we are queuing the previous one with their scores. The best ones are at the bottom of the queue, the worst ones are at the top of the queue.
 
**_c.	Crossovering The Genoms_**

![resim](https://user-images.githubusercontent.com/61044813/124250326-73ef3580-db2d-11eb-80a7-b3fbed147bdb.png)


We decided the best ones according to their individual scores. So, the bird that has the best score is more likely to be selected. But there is a probability to select the worst one, so it is acceptable, because the worst one can do the right movements also. To give a chance to the worst scored bird we choose 2 parents and crossover them with a randomly selected crossover point to produce 2 babies. (figure 3). There is a probability to be mutated for baby birds that come from crossover.

### Result and Performance

After every epoch(generation) the birds are generally flying better. Averagely after 6-7 epochs, at least 1 bird flies perfectly. Although rarely if the height of the pipes on Y coordinate is irregular, agents may not learn to move up or down. That may sometimes cause the infinite loop. (Agent may try to always move upward or downward.) In this case the game should be reset from the menu of the game to reset learning of the neurons. Also, height of the pipe can be decreased on the editor from the following steps:
Scenes -> Game -> Hierarchy -> GameManger -> Height
 (Pipes are constructed on Y coordinate with the random range of height given on the editor. (-height, height)). This solution makes the learning process faster but makes the game easier, so if the game becomes easy agents may not learn efficiently.
After these considerations it can be observed that one of the neurons can play a big role for learning. So, it is crucial to choose the inputs efficiently and the data that a regular person has can be enough to approach the best solution.
