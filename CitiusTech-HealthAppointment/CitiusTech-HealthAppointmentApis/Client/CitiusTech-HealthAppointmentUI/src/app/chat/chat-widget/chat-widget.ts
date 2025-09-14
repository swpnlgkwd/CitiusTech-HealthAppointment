import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, ElementRef, NgZone, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SmartSuggestion } from '../../core/models/smart-suggestion.model';
import { AgentService } from '../../core/services/agent';
import { SmartSuggestionsService } from '../../core/services/smart-suggestion';
import { QuickReply } from '../../core/models/agent-daily-summary.model';

interface ChatMessage {
  text: string;
  sender: 'user' | 'bot';
  time: string;
  quickReplies?: QuickReply[];
}

@Component({
  selector: 'app-chat-widget',
  templateUrl: './chat-widget.html',
  styleUrls: ['./chat-widget.css'],
  imports: [CommonModule, ReactiveFormsModule],
  standalone: true
})

export class ChatWidgetComponent implements OnInit {
  @ViewChild('chatContainer') chatContainer!: ElementRef;
  isOpen = false;
  chatForm: FormGroup;
  isBotTyping = true;
  messages: ChatMessage[] = [];

  constructor(private fb: FormBuilder, private agentService: AgentService, private smartSuggestionService: SmartSuggestionsService,
    private ngZone: NgZone, private cdRef: ChangeDetectorRef) {
    this.chatForm = this.fb.group({
      message: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadSmartSuggestions();
  }

  toggleChat(): void {
    this.isOpen = !this.isOpen;
  }

  refreshChat(): void {
    this.messages = [];
    this.agentService.refresh().subscribe({
      next: (response) => {
        console.log(response);
        console.log(response.threadId);
        localStorage.setItem("threadId", response.threadId)
      },
      error: (error) => {
        console.error('Agent error:', error);
      }
    });
  }

  private getCurrentTime(): string {
    return new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }

  scrollToBottom(): void {
    setTimeout(() => {
      try {
        this.chatContainer.nativeElement.scrollTop = this.chatContainer.nativeElement.scrollHeight;
      } catch (e) {
        console.error('Scroll error:', e);
      }
    }, 100);
  }

  async sendMessage(action?: string): Promise<void> {
    if (this.chatForm.invalid && !action) return;

    const userMessage = action ?? this.chatForm.value.message;
    const currentTime = new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

    // Push user message
    this.messages.push({ sender: 'user', text: userMessage, time: currentTime });
    this.scrollToBottom();

    this.removePreviousQuickReplies();

    this.chatForm.reset();

    // Show typing indicator
    this.isBotTyping = true;

    this.agentService.askAgent(userMessage).subscribe({
      next: async (response) => {
        this.messages.push({
          sender: 'bot',
          text: response.reply || '🤖 (No reply)',
          time: this.getCurrentTime(),
          quickReplies: []
        });
        await this.playAudio();
        this.isBotTyping = false;
        this.cdRef.detectChanges();
        this.scrollToBottom();
        document.getElementById('messageInput')?.focus();
      },
      error: (error) => {
        console.error('Agent error:', error);
        this.isBotTyping = false;
        this.messages.push({
          sender: 'bot',
          text: '⚠️ Something went wrong. Please try again later.',
          time: this.getCurrentTime()
        });
      }
    });
  }

  handleActionClick(s: SmartSuggestion): void {
    const actionMessage = {
      type: s.type,
      actionName: s.actionName,
      data: s.actionData || {}
    };

    this.sendMessage(s.actionText); // This should route to your chat API
  }

  handleQuickReply(reply: string) {

    // Simulate bot typing
    this.isBotTyping = true;

    this.sendMessage(reply);

    this.scrollToBottom();
    this.removePreviousQuickReplies()
  }


  async loadSmartSuggestions(): Promise<void> {

    // Show typing animation first
    this.isBotTyping = true;

    // On init or scheduler landing
    this.smartSuggestionService.getDailySummary().subscribe({
      next: async (response) => {
        const message = response?.summaryMessage?.trim();

        if (message) {
          this.messages.push({
            sender: 'bot',
            text: message,
            time: this.getCurrentTime(),
            quickReplies: response.quickReplies
          });
        } else {
          this.messages.push({
            sender: 'bot',
            time: this.getCurrentTime(),
            text: `Welcome to MediMate! How can I assist you today?`
          });
        }
        this.isOpen = !this.isOpen ? true : this.isOpen; // Open chat if not already open
        await this.playAudio();
        this.isBotTyping = false;
        this.cdRef.detectChanges();
      },
      error: (err: any) => {
        console.error('Failed to load daily agent summary', err);
        this.messages.push({
          sender: 'bot',
          time: this.getCurrentTime(),
          text: `Welcome to MediMate! How can I assist you today?`
        });
        this.isBotTyping = false;
        this.cdRef.detectChanges();
      }
    });
  }

  removePreviousQuickReplies(): void {
    // Only remove quick replies from the last bot message (if any)
    for (let i = this.messages.length - 1; i >= 0; i--) {
      if (this.messages[i].sender === 'bot' && this.messages[i].quickReplies) {
        delete this.messages[i].quickReplies;
        break;
      }
    }
  }

  async playAudio() {
    console.log('Play Audio button clicked'); // Debugging line
    var audio = document.getElementById("myAudio") as HTMLAudioElement;
    if (audio) {
      await audio.play().then(() => {
      }).catch((error) => {
        console.error('Audio play failed', error);
      });
    } else {
      console.log('Audio element not found');
    }
  }
}
